using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Grouping;
using Lucene.Net.Util;
using PiranhaCMS.Search.Extensions;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Models.Facets;
using PiranhaCMS.Search.Models.Internal;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

internal class DocumentReader : IDocumentReader
{
    public DirectoryReader? Reader => _reader;
    private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
    private DirectoryReader? _reader;
    private DirectoryTaxonomyReader? _taxoReader;
    private Analyzer? _analyzer;
    private readonly string _indexName;
    private bool _isInitialized = false;
    private bool _hasFacets = false;
    private readonly FacetsConfig _facetsConfig;
    private readonly string _id;
    private bool _disposed = false;

    public DocumentReader(
        Lucene.Net.Store.Directory directory,
        Lucene.Net.Store.Directory taxoDirectory,
        string indexName,
        FacetsConfig facetsConfig,
        bool hasFacets = false,
        string idField = "id")
    {
        _indexName = indexName ?? "index";
        _hasFacets = hasFacets;
        _facetsConfig = facetsConfig;
        _id = idField;

        Init(directory, taxoDirectory);
    }

    public bool DocumentExists(string id)
    {
        return _reader is null
            ? false
            : _reader.DocFreq(new Term(_id, id)) != 0;
    }

    public IDictionary<string, int> TermsCounter(string field, bool isNumeric = false)
    {
        var res = new Dictionary<string, int>();
        var searcher = new IndexSearcher(_reader);
        var fields = MultiFields.GetFields(searcher.IndexReader);
        var terms = fields.GetTerms(field);
        var termsEnum = terms.GetEnumerator();

        while (termsEnum.MoveNext() == true)
        {
            var collector = new TotalHitCountCollector();
            searcher.CollectionStatistics(field);
            searcher.Search(new TermQuery(new Term(field, termsEnum.Term)), collector);

            if (collector.TotalHits > 0)
                if (isNumeric)
                {
                    if (!res.ContainsKey(NumericUtils.PrefixCodedToInt32(termsEnum.Term).ToString()) &&
                        searcher.Search(NumericRangeQuery.NewInt32Range(field, NumericUtils.PrefixCodedToInt32(termsEnum.Term), NumericUtils.PrefixCodedToInt32(termsEnum.Term), true, true), 1).TotalHits > 0)
                        res.Add(NumericUtils.PrefixCodedToInt32(termsEnum.Term).ToString(), collector.TotalHits);
                }
                else if (!res.ContainsKey(termsEnum.Term.Utf8ToString()))
                    res.Add(termsEnum.Term.Utf8ToString(), collector.TotalHits);
        }

        return res;
    }

    public IDictionary<string, string> LatestAdded(string field, string additionalField, string sortBy, ListSortDirection sortDirection, int top)
    {
        var sort = new Sort(new SortField(sortBy, SortFieldType.INT64, sortDirection.Equals(ListSortDirection.Descending)));
        var res = new ConcurrentDictionary<string, string>();
        var searcher = new IndexSearcher(_reader);
        var query = new MatchAllDocsQuery();
        var groupingSearch = new GroupingSearch(field);
        groupingSearch.SetGroupSort(sort);
        //groupingSearch.SetFillSortFields(true);
        groupingSearch.SetGroupDocsLimit(1);
        var groupingDocs = groupingSearch.SearchByField(searcher, query, 0, top);

        groupingDocs.Groups
            .AsParallel()
            .ForAll(x => res.AddOrUpdate(x.GroupValue.Utf8ToString(), searcher.Doc(x.ScoreDocs[0].Doc).Get(additionalField), (newValue, existingValue) => newValue));

        return res;
    }

    public IEnumerable<Document> GetByIds(string[] ids)
    {
        if (ids == null || ids.Length == 0)
            yield return [];

        var searcher = new IndexSearcher(_reader);

        TermQuery q;

        for (int i = 0; i < ids.Length; i++)
        {
            string? id = ids[i];
            q = new TermQuery(new Term(_id, id));

            var res = searcher.Search(q, 1);
            if (res.TotalHits == 0) continue;

            yield return searcher.Doc(res.ScoreDocs[0].Doc);
        }
    }

    public bool IndexNotExistsOrEmpty()
    {
        return !DirectoryReader.IndexExists(_reader?.Directory);
    }

    public SearchResultInternal Search(SearchRequestInternal request)
    {
        var searcher = new IndexSearcher(_reader);
        var searchResult = new SearchResultInternal
        {
            SearchRequest = request,
            SearchParam = request.SearchFields != null && request.SearchFields.Any()
            ? request.SearchFields.First().Name
            : "q",
            SearchText = request.SearchFields.First().Value,
            Hits = []
        };
        Query? q = null;
        Query? facetsQuery = null;

        switch (request.QueryType)
        {
            case QueryTypesEnum.Term:
                if (request.SearchFields is null || request.SearchFields.Count() == 0)
                    break;

                q = new TermQuery(new Term(request.SearchFields.First().Name, request.SearchFields.First().Value));
                facetsQuery = q;
                break;
            case QueryTypesEnum.MultiTerm:
                q = new BooleanQuery();
                facetsQuery = new BooleanQuery();

                if (request.SearchFields is null)
                    break;

                foreach (var field in request.SearchFields)
                {
                    if (field.Properties.FieldType == FieldTypeEnum.Int32Field)
                    {
                        ((BooleanQuery)q).Add(NumericRangeQuery.NewInt32Range(field.Name, int.Parse(field.Value), int.Parse(field.Value), true, true), Occur.MUST);
                        if (request.Facets.Any(x => x.Key.Equals(field.Name)))
                            ((BooleanQuery)facetsQuery).Add(NumericRangeQuery.NewInt32Range(field.Name, int.Parse(field.Value), int.Parse(field.Value), true, true), Occur.MUST);
                    }
                    else
                    {
                        Query searchQuery = field.SearchType switch
                        {
                            SearchType.ExactMatch => new TermQuery(new Term(field.Name, field.Value)),
                            SearchType.PrefixMatch => new PrefixQuery(new Term(field.Name, field.Value)),
                            SearchType.FuzzyMatch => new FuzzyQuery(new Term(field.Name, field.Value)),
                            SearchType.QueryMatch => new QueryParser(AppLuceneVersion, request.SearchFields.First().Name, _analyzer)
                            {
                                AllowLeadingWildcard = true,
                                DefaultOperator = Operator.AND
                            }.Parse(field.Value),
                            _ => new TermQuery(new Term(field.Name, field.Value))
                        };
                        ((BooleanQuery)q).Add(searchQuery, Occur.MUST);

                        if (request.Facets.Any(x => x.Key.Equals(field.Name)))
                            ((BooleanQuery)facetsQuery).Add(searchQuery, Occur.MUST);
                    }
                }
                //facetsQuery = q;
                break;
            case QueryTypesEnum.Numeric:
                if (request.SearchFields is null || request.SearchFields.Count() == 0)
                    break;

                q = NumericRangeQuery.NewInt32Range(request.SearchFields.First().Name, int.Parse(request.SearchFields.First().Value), int.Parse(request.SearchFields.First().Value), true, true);
                facetsQuery = q;
                break;
            case QueryTypesEnum.Text:
                var parser = new QueryParser(AppLuceneVersion, request.SearchFields.First().Name, _analyzer)
                {
                    AllowLeadingWildcard = true,
                    DefaultOperator = Operator.AND
                };
                q = parser.Parse(request.SearchFields.First().Value);
                facetsQuery = q;
                break;
        }

        if (request.Facets != null && request.Facets.Any() && facetsQuery is not null)
            searchResult.Facets = GetFacets(searcher, facetsQuery, request);

        var startIndex = request.Pagination.PageIndex * request.Pagination.PageSize;
        var sort = new Sort(
            new SortField("art", SortFieldType.STRING, false),
            new SortField("yer", SortFieldType.INT32, false),
            new SortField("fnm", SortFieldType.STRING, false));
        var topDocs = searcher.Search(q, startIndex + request.Pagination.PageSize, sort);

        if (topDocs.TotalHits == 0) return searchResult;

        var hits = new ConcurrentBag<Document>();

        Parallel.ForEach(topDocs.ScoreDocs.Skip(startIndex), hit =>
        {
            hits.Add(searcher.Doc(hit.Doc));
        });

        searchResult.TotalHits = topDocs.TotalHits;
        searchResult.Hits = hits;
        searchResult.Pagination = new PaginationDto(
            request.Pagination.PageSize,
            request.Pagination.PageIndex,
            (int)Math.Ceiling(decimal.Divide(searchResult.TotalHits, request.Pagination.PageSize)),
            request.Pagination.QueryString);

        return searchResult;
    }

    public void Init(Lucene.Net.Store.Directory directory, Lucene.Net.Store.Directory taxoDirectory)
    {
        if (_isInitialized)
            return;

        //var path = Path.Combine(Environment.CurrentDirectory, "Index", _indexName);

        //if (!Directory.Exists(path.ToString()))
        //    return;

        _analyzer = new WhitespaceAnalyzer(AppLuceneVersion);
        _reader = DirectoryReader.Open(directory);

        //var pathTaxo = path + "-taxo";

        //if (_hasFacets && Directory.Exists(pathTaxo) && Directory.GetFiles(pathTaxo).Length > 0)
        _taxoReader = new DirectoryTaxonomyReader(taxoDirectory);

        _isInitialized = true;
    }

    private FacetFilter[] GetFacets(IndexSearcher searcher, Query q, SearchRequestInternal request)
    {
        if (_facetsConfig == null)
            return [];

        var fc = new FacetsCollector();
        var sort = new Sort(
            new SortField("yer", SortFieldType.INT32, false),
            new SortField("rel", SortFieldType.STRING, false));

        FacetsCollector.Search(searcher, q, null, 1, sort, fc);

        //TODO Append year to release facet value
        return new FastTaxonomyFacetCounts(_taxoReader, _facetsConfig, fc)
            .GetAllDims(100)
            .AsParallel()
            .Select(facet => facet.ToFacetFilter(request.Pagination.QueryString))
            .ToArray();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _reader?.Dispose();
            _taxoReader?.Dispose();
            _analyzer?.Dispose();
        }

        _disposed = true;
    }
}
