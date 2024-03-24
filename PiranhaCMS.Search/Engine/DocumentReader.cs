﻿using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Grouping;
using Lucene.Net.Store;
using Lucene.Net.Util;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Models.Facets;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

internal class DocumentReader : IDisposable, IDocumentReader
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
    private readonly string _sharedIndexName;

    public DocumentReader(string indexName, FacetsConfig facetsConfig, bool hasFacets = false, string idField = "id", string sharedIndexName = "")
    {
        _indexName = indexName ?? "index";
        _hasFacets = hasFacets;
        _facetsConfig = facetsConfig;
        _id = idField;
        _sharedIndexName = sharedIndexName;
    }

    public bool DocumentExists(string id)
    {
        return _reader is null ? false : _reader.DocFreq(new Term(_id, id)) != 0;
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
        var res = new Dictionary<string, string>();
        var searcher = new IndexSearcher(_reader);
        var query = new MatchAllDocsQuery();
        var groupingSearch = new GroupingSearch(field);
        groupingSearch.SetGroupSort(sort);
        //groupingSearch.SetFillSortFields(true);
        groupingSearch.SetGroupDocsLimit(1);
        var groupingDocs = groupingSearch.SearchByField(searcher, query, 0, top);

        foreach (var item in groupingDocs.Groups)
            res.Add(item.GroupValue.Utf8ToString(), searcher.Doc(item.ScoreDocs[0].Doc).Get(additionalField));

        return res;
    }

    public IEnumerable<Document> GetByIds(string[] ids)
    {
        if (ids == null || ids.Length == 0)
            return [];

        var hits = new Collection<Document>();
        var searcher = new IndexSearcher(_reader);

        TermQuery q;

        foreach (var id in ids)
        {
            q = new TermQuery(new Term(_id, id));

            var res = searcher.Search(q, 1);
            if (res.TotalHits == 0) continue;

            hits.Add(searcher.Doc(res.ScoreDocs[0].Doc));
        }

        return hits;
    }

    public bool IndexNotExistsOrEmpty()
    {
        return _reader is null ? true : _reader.NumDocs == 0;
    }

    public SearchResult Search(SearchRequest request)
    {
        var searcher = new IndexSearcher(_reader);
        var searchResult = new SearchResult
        {
            SearchParam = request.SearchFields != null && request.SearchFields.Any()
            ? request.SearchFields.First().Key
            : "q",
            SearchText = request.Text ?? request.SearchFields.First().Value,
            Hits = []
        };
        Query? q = null;
        Query? facetsQuery = null;

        switch (request.QueryType)
        {
            case QueryTypesEnum.Term:
                if (request.SearchFields is null || request.SearchFields.Count == 0)
                    break;

                q = new TermQuery(new Term(request.SearchFields.First().Key, request.SearchFields.First().Value));
                facetsQuery = q;
                break;
            case QueryTypesEnum.MultiTerm:
                q = new BooleanQuery();

                if (request.SearchFields is null) break;

                foreach (var (fieldName, value) in request.SearchFields)
                {
                    Query searchQuery = request.SearchType switch
                    {
                        SearchType.ExactMatch => new TermQuery(new Term(fieldName, value)),
                        SearchType.PrefixMatch => new PrefixQuery(new Term(fieldName, value)),
                        SearchType.FuzzyMatch => new FuzzyQuery(new Term(fieldName, value)),
                        _ => new TermQuery(new Term(fieldName, value))
                    };

                    ((BooleanQuery)q).Add(searchQuery, Occur.MUST);
                }
                facetsQuery = new TermQuery(new Term(request.SearchFields.First().Key, request.SearchFields.First().Value));
                break;
            case QueryTypesEnum.Numeric:
                if (request.SearchFields is null || request.SearchFields.Count == 0)
                    break;

                q = NumericRangeQuery.NewInt32Range(request.SearchFields.First().Key, int.Parse(request.SearchFields.First().Value), int.Parse(request.SearchFields.First().Value), true, true);
                facetsQuery = q;
                break;
            case QueryTypesEnum.Text:
                var parser = new QueryParser(AppLuceneVersion, request.SearchFields.First().Key, _analyzer)
                {
                    AllowLeadingWildcard = true,
                    DefaultOperator = Operator.AND
                };
                q = parser.Parse(request.Text);
                facetsQuery = q;
                break;
        }

        if (request.Facets != null && request.Facets.Any() && facetsQuery is not null)
            searchResult.Facets = GetFacets(searcher, facetsQuery);

        var startIndex = request.Pagination.PageIndex * request.Pagination.PageSize;
        var sort = new Sort(
            new SortField("art", SortFieldType.STRING, false),
            new SortField("yer", SortFieldType.INT32, false),
            new SortField("fnm", SortFieldType.STRING, false));
        var topDocs = searcher.Search(q, startIndex + request.Pagination.PageSize, sort);

        if (topDocs.TotalHits == 0) return searchResult;

        var hits = new ConcurrentBag<Document>();

        Parallel.ForEach(topDocs.ScoreDocs.Skip(startIndex), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, hit =>
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

    public void Init()
    {
        if (_isInitialized)
            return;

        var path = Path.Combine(Environment.CurrentDirectory, "Index", _indexName);

        if (!System.IO.Directory.Exists(path.ToString()))
            return;

        _analyzer = new WhitespaceAnalyzer(AppLuceneVersion);
        _reader = DirectoryReader.Open(FSDirectory.Open(path.ToString()));

        var pathTaxo = path + "-taxo";

        if (_hasFacets && System.IO.Directory.Exists(pathTaxo) && System.IO.Directory.GetFiles(pathTaxo).Length > 0)
            _taxoReader = new DirectoryTaxonomyReader(FSDirectory.Open(pathTaxo));

        _isInitialized = true;
    }

    public void Init(DirectoryReader reader)
    {
        _reader = reader;
    }

    private IEnumerable<FacetFilter> GetFacets(IndexSearcher searcher, Query q)
    {
        if (_facetsConfig == null)
            return [];

        var fc = new FacetsCollector();
        var sort = new Sort(
            new SortField("art", SortFieldType.STRING, false),
            new SortField("yer", SortFieldType.INT32, false),
            new SortField("fnm", SortFieldType.STRING, false));
        FacetsCollector.Search(searcher, q, null, 100, sort, fc);
        var facets = new FastTaxonomyFacetCounts(_taxoReader, _facetsConfig, fc);
        var facetResults = facets.GetAllDims(100)
            .Select(facet => new FacetFilter
            {
                Name = facet.Dim,
                Values = facet.LabelValues.Select(p => new FacetValue { Value = p.Label, Count = (int)p.Value, })
            });

        return facetResults;
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _taxoReader?.Dispose();
        _analyzer?.Dispose();
    }
}
