using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Util;
using PiranhaCMS.Search.ModelBuilders;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;
using PiranhaCMS.Search.Providers;
using PiranhaCMS.Search.Startup;

namespace PiranhaCMS.Search.Engine;

public class SearchIndexEngine : ISearchIndexEngine
{
    #region Private fields

    private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
    private readonly FacetsConfig _facetsConfig;
    private readonly Lucene.Net.Store.Directory _directory;
    private readonly Lucene.Net.Store.Directory _taxoDirectory;
    private readonly Analyzer _analyzer;

    private IndexWriter IndexWriter => new(_directory, new IndexWriterConfig(AppLuceneVersion, _analyzer));

    #endregion

    #region Constructors

    public SearchIndexEngine(PiranhaSearchServiceBuilder serviceBuilder)
    {
        _directory = DirectoryProvider.CreateDocumentIndex(serviceBuilder);
        _taxoDirectory = DirectoryProvider.CreateFacetIndex(serviceBuilder);
        _facetsConfig = new FacetsConfig();
        _facetsConfig.SetHierarchical(FieldNames.Category, true);

        _analyzer = serviceBuilder.DefaultAnalyzer switch
        {
            Models.Enums.DefaultAnalyzer.English => new StandardAnalyzer(AppLuceneVersion),
            Models.Enums.DefaultAnalyzer.Other => new WhitespaceAnalyzer(AppLuceneVersion),
            _ => new StandardAnalyzer(AppLuceneVersion),
        };
    }

    #endregion

    #region Public methods

    public void AddToIndex(WebPage webPage)
    {
        var document = DocumentModelBuilders.BuildWebPageDocument(webPage);

        using var writer = IndexWriter;
        writer.AddDocument(document);
        writer.UpdateDocument(new Term(FieldNames.Url, webPage.Url), document);
        writer.Flush(false, false);
    }

    public void AddToIndex(Content content)
    {
        var document = DocumentModelBuilders.BuildContentDocument(content);

        using var writer = IndexWriter;
        writer.AddDocument(document);
        writer.UpdateDocument(new Term(FieldNames.Id, content.ContentId), document);
        writer.Flush(false, false);
    }

    public void AddToIndexWithoutCommit(WebPage webPage)
    {
        var document = DocumentModelBuilders.BuildWebPageDocument(webPage);

        using var writer = IndexWriter;
        writer.AddDocument(document);
        writer.UpdateDocument(new Term(FieldNames.Url, webPage.Url), document);
    }

    public void AddToIndexWithoutCommit(Content content)
    {
        var document = DocumentModelBuilders.BuildContentDocument(content);

        using var writer = IndexWriter;
        using var taxoWriter = new DirectoryTaxonomyWriter(_taxoDirectory);
        writer.AddDocument(_facetsConfig.Build(taxoWriter, DocumentModelBuilders.BuildFacetDocument(content)));
        writer.AddDocument(document);
        writer.UpdateDocument(new Term(FieldNames.Id, content.ContentId), document);
    }

    public SearchResult Search(SearchRequest request)
    {
        using var indexReader = DirectoryReader.Open(_directory);
        using var taxoReader = new DirectoryTaxonomyReader(_taxoDirectory);
        var searcher = new IndexSearcher(indexReader);
        var searchResult = new SearchResult
        {
            SearchText = request.Text,
            Hits = []
        };

        if (string.IsNullOrEmpty(request.Text))
        {
            var getAllQuery = new MatchAllDocsQuery();
            searcher.Search(getAllQuery, 10);
        }

        var boosts = new Dictionary<string, float>
        {
            { FieldNames.Text, 1 },
            { FieldNames.Title, 2 },
            { FieldNames.Category, 3 }
        };

        var parser = new MultiFieldQueryParser(
            AppLuceneVersion,
            boosts.Select(x => x.Key).ToArray(),
            _analyzer,
            boosts)
        {
            AllowLeadingWildcard = true,
            DefaultOperator = Operator.AND
        };

        var q = parser.Parse(request.Text);

        #region Text highlighter

        var queryScorer = new QueryScorer(q, FieldNames.Text);
        var highlighter = new Highlighter(queryScorer);
        highlighter.TextFragmenter = new SimpleSpanFragmenter(queryScorer);

        #endregion

        #region Facets collector

        var fc = new FacetsCollector();
        searcher.Search(q, null, fc);
        FacetsCollector.Search(searcher, q, 50, fc);

        var facets = new FastTaxonomyFacetCounts(taxoReader, _facetsConfig, fc);
        var results = new List<FacetResult>
        {
            facets.GetTopChildren(10, FieldNames.Category)
        };

        #endregion

        var startIndex = request.Pagination.PageIndex * request.Pagination.PageSize;
        var topDocs = searcher.Search(q, startIndex + request.Pagination.PageSize);

        if (topDocs.TotalHits == 0) return searchResult;

        var hits = new List<SearchHit>(topDocs.ScoreDocs.Skip(startIndex).Count());

        foreach (var hit in topDocs.ScoreDocs.Skip(startIndex))
        {
            var documentId = hit.Doc;
            var document = searcher.Doc(documentId);
            var tokenStream = TokenSources.GetAnyTokenStream(
                indexReader,
                hit.Doc,
                FieldNames.Text,
                document,
                _analyzer);
            var searchHit = new SearchHit
            {
                Url = document.Get(FieldNames.Url),
                Title = document.Get(FieldNames.Title),
                HighlightedText = highlighter.GetBestFragment(tokenStream, document.Get(FieldNames.Text)),
                Score = hit.Score
            };

            hits.Add(searchHit);
        }

        searchResult.TotalHits = topDocs.TotalHits;
        searchResult.Hits = hits;
        searchResult.Pagination = request.Pagination;

        return searchResult;
    }

    public void Commit()
    {
        using var writer = IndexWriter;
        writer.Commit();
    }

    public void DeleteAll()
    {
        using var writer = IndexWriter;
        writer.DeleteAll();
    }

    public void DeleteById(string id)
    {
        using var writer = IndexWriter;
        writer.DeleteDocuments(new TermQuery(new Term(FieldNames.Id, id)));
    }

    public bool IndexExists()
    {
        return DirectoryReader.IndexExists(_directory);
    }

    #endregion
}
