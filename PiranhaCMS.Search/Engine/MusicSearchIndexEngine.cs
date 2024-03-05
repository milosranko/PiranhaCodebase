using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Providers;
using System.Collections.ObjectModel;

namespace PiranhaCMS.Search.Engine;

public class MusicSearchIndexEngine : IMusicSearchIndexEngine
{
    private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
    private readonly Analyzer _analyzer;
    private readonly string _indexName;

    public string GetIndexName()
    {
        return _indexName;
    }

    public MusicSearchIndexEngine(string indexName)
    {
        _indexName = indexName;
        _analyzer = new WhitespaceAnalyzer(AppLuceneVersion);
    }

    public MusicSearchResult Search(MusicSearchRequest request)
    {
        using var directory = DirectoryProvider.GetMusicDocumentIndex(_indexName);
        using var reader = DirectoryReader.Open(directory);
        var searcher = new IndexSearcher(reader);
        var searchResult = new MusicSearchResult
        {
            SearchText = request.Text,
            Hits = Enumerable.Empty<MusicSearchHit>()
        };

        Query q = null;

        switch (request.QueryType)
        {
            case QueryTypesEnum.Term:
                q = new TermQuery(new Term(request.Fields[0], request.Text));
                break;
            case QueryTypesEnum.MultiTerm:
                q = new BooleanQuery();

                if (request.Terms.Length.Equals(request.Fields.Length) && request.Terms.Length > 0)
                    for (int i = 0; i < request.Terms.Length; i++)
                        ((BooleanQuery)q).Add(new TermQuery(new Term(request.Fields[i], request.Terms[i])), Occur.MUST);
                break;
            case QueryTypesEnum.Numeric:
                q = NumericRangeQuery.NewInt32Range(request.Fields[0], int.Parse(request.Text), int.Parse(request.Text), true, true);
                break;
            case QueryTypesEnum.Text:
                var parser = new QueryParser(AppLuceneVersion, request.Fields[0], _analyzer)
                {
                    AllowLeadingWildcard = true,
                    DefaultOperator = Operator.AND
                };
                q = parser.Parse(request.Text);
                break;
        }

        var startIndex = request.Pagination.PageIndex * request.Pagination.PageSize;
        var topDocs = searcher.Search(q, startIndex + request.Pagination.PageSize);

        if (topDocs.TotalHits == 0) return searchResult;

        var hits = new List<MusicSearchHit>(topDocs.ScoreDocs.Skip(startIndex).Count());

        foreach (var hit in topDocs.ScoreDocs.Skip(startIndex))
            hits.Add(CreateSearchHit(searcher.Doc(hit.Doc)));

        searchResult.TotalHits = topDocs.TotalHits;
        searchResult.Hits = hits.OrderBy(x => x.Name).ToList();
        searchResult.Pagination = request.Pagination;

        return searchResult;
    }

    public bool IndexNotExistsOrEmpty()
    {
        using var directory = DirectoryProvider.GetMusicDocumentIndex(_indexName);
        if (!DirectoryReader.IndexExists(directory)) return true;

        using var reader = DirectoryReader.Open(directory);
        return reader.NumDocs == 0;
    }

    public MusicIndexCounts GetIndexStatistics()
    {
        using var directory = DirectoryProvider.GetMusicDocumentIndex(_indexName);
        using var reader = DirectoryReader.Open(directory);
        var searcher = new IndexSearcher(reader);

        return new MusicIndexCounts
        {
            TotalFiles = reader.NumDocs,
            TotalFilesByExtension = GetMostFrequentTerms(searcher, FieldNames.Extension),
            TotalHiResFiles = Search(new MusicSearchRequest
            {
                Text = QueryParser.Escape("hr flac"),
                Fields = [FieldNames.Text],
                QueryType = QueryTypesEnum.Text,
                Terms = null,
                Pagination = new Pagination(100000, 0)
            }).TotalHits,
            ReleaseYears = GetMostFrequentTermsNumeric(searcher, FieldNames.Year),
            GenreCount = GetMostFrequentTerms(searcher, FieldNames.Genre),
            LatestAdditions = GetLatestAddedItems(searcher, FieldNames.ModifiedDate, 500)
        };
    }

    private ICollection<ValueTuple<string, string>> GetLatestAddedItems(IndexSearcher searcher, string field, int count)
    {
        var res = new Collection<ValueTuple<string, string>>();
        var query = new MatchAllDocsQuery();
        var sort = new Sort(new SortField(field, SortFieldType.INT64, true));
        var topDocs = searcher.Search(query, count, sort);
        string artist, release, folder, prevFolder = null;

        for (int i = 0; i < topDocs.ScoreDocs.Length; i++)
        {
            folder = Path.GetDirectoryName(searcher.Doc(topDocs.ScoreDocs[i].Doc).Get(FieldNames.Id));
            artist = searcher.Doc(topDocs.ScoreDocs[i].Doc).Get(FieldNames.Artist);
            release = searcher.Doc(topDocs.ScoreDocs[i].Doc).Get(FieldNames.Album);

            if (!res.Contains((artist, release)) && prevFolder != folder)
                res.Add((artist, release));

            if (res.Count == 50)
                break;

            if (prevFolder != folder)
                prevFolder = folder;
        }

        return res;
    }

    private IDictionary<string, int> GetMostFrequentTerms(IndexSearcher searcher, string field)
    {
        var res = new Dictionary<string, int>();
        var fields = MultiFields.GetFields(searcher.IndexReader);
        var terms = fields.GetTerms(field);
        var termsEnum = terms.GetEnumerator(null);

        while (termsEnum.MoveNext() == true)
        {
            var collector = new TotalHitCountCollector();
            searcher.Search(new TermQuery(new Term(field, termsEnum.Term)), collector);

            if (collector.TotalHits > 0)
                res.Add(termsEnum.Term.Utf8ToString(), collector.TotalHits);
        }

        return res;
    }

    private IDictionary<string, int> GetMostFrequentTermsNumeric(IndexSearcher searcher, string field)
    {
        var res = new Dictionary<string, int>();
        var fields = MultiFields.GetFields(searcher.IndexReader);
        var terms = fields.GetTerms(field);
        var termsEnum = terms.GetEnumerator(null);
        int term;

        while (termsEnum.MoveNext() == true)
        {
            var collector = new TotalHitCountCollector();
            searcher.Search(new TermQuery(new Term(field, termsEnum.Term)), collector);
            term = NumericUtils.PrefixCodedToInt32(termsEnum.Term);

            if (!res.ContainsKey(term.ToString()) &&
                collector.TotalHits > 0 &&
                term > 1921)
                res.Add(term.ToString(), collector.TotalHits);
        }

        return res;
    }

    private MusicSearchHit CreateSearchHit(Document doc)
    {
        var id = doc.Get(FieldNames.Id);

        return new MusicSearchHit
        {
            Id = id,
            Name = GetFileNameWithoutExtension(id),
            Tags = string.Join("|", doc.GetValues(FieldNames.Tags))
        };
    }

    private string GetFileNameWithoutExtension(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.Split(@"\", StringSplitOptions.RemoveEmptyEntries).Last();
    }
}
