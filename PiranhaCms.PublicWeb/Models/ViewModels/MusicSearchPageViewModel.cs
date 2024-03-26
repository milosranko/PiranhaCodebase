using Piranha.Cache;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Extensions;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Models.Requests;
using System.Text;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record MusicSearchPageViewModel : PageViewModel<MusicSearchPage>
{
    public const int PageSize = 20;
    public SearchResultDto<MusicLibraryDocument> SearchResult { get; private set; }
    public MusicIndexCounts IndexCounts { get; private set; }

    public MusicSearchPageViewModel(
        MusicSearchPage currentPage,
        HttpRequest request,
        ISearchIndexEngine<MusicLibraryDocument> engine,
        ICache cache) : base(currentPage)
    {
        SearchResult = SearchResultDto<MusicLibraryDocument>.Empty();
        IndexCounts = MusicIndexCounts.Empty;

        var searchText = request.Query["q"].ToString();
        var artist = request.Query[engine.GetFieldName(x => x.Artist)].ToString();
        var release = request.Query[engine.GetFieldName(x => x.Release)].ToString();
        var genre = request.Query[engine.GetFieldName(x => x.Genre)].ToString();
        var year = request.Query[engine.GetFieldName(x => x.Year)].ToString();
        int.TryParse(request.Query["page"], out int pageIndex);
        var paginationQueryString = new StringBuilder();

        if (!string.IsNullOrEmpty(searchText))
        {
            paginationQueryString.Append("?q=");
            paginationQueryString.Append(searchText);

            SearchResult = PerformSearch(
            engine,
            searchText,
            [],
            [engine.GetFieldName(x => x.Text)],
            QueryTypesEnum.Text,
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(release) && !string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append($"?{engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);
            paginationQueryString.Append($"&{engine.GetFieldName(x => x.Release)}=");
            paginationQueryString.Append(release);

            SearchResult = PerformSearch(
            engine,
            default,
            [artist, release],
            [engine.GetFieldName(x => x.Artist), engine.GetFieldName(x => x.Release)],
            QueryTypesEnum.MultiTerm,
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(genre))
        {
            paginationQueryString.Append($"?{engine.GetFieldName(x => x.Genre)}=");
            paginationQueryString.Append(genre);

            SearchResult = PerformSearch(
            engine,
            default,
            [genre],
            [engine.GetFieldName(x => x.Genre)],
            QueryTypesEnum.Term,
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(year))
        {
            paginationQueryString.Append($"?{engine.GetFieldName(x => x.Year)}=");
            paginationQueryString.Append(year);

            SearchResult = PerformSearch(
            engine,
            default,
            [year],
            [engine.GetFieldName(x => x.Year)],
            QueryTypesEnum.Numeric,
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { engine.GetFieldName(x => x.Artist), [] }, { engine.GetFieldName(x => x.Release), [] } });
        }
        else if (!string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append($"?{engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);

            SearchResult = PerformSearch(
            engine,
            default,
            [artist],
            [engine.GetFieldName(x => x.Artist)],
            QueryTypesEnum.Term,
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { engine.GetFieldName(x => x.Release), [] } });
        }
        else
        {
            var cachedObject = cache.Get<MusicIndexCounts>(CacheKeys.MusicIndexCount);
            if (cachedObject != null)
            {
                IndexCounts = cachedObject;
            }
            else
            {
                var indexCounts = GetIndexCounts(engine);
                cache.Set(CacheKeys.MusicIndexCount, indexCounts);
                IndexCounts = indexCounts;
            }
        }
    }

    private MusicIndexCounts GetIndexCounts(ISearchIndexEngine<MusicLibraryDocument> searchIndexEngine)
    {
        if (searchIndexEngine.IndexNotExistsOrEmpty())
            return MusicIndexCounts.Empty;

        return new MusicIndexCounts
        {
            TotalFiles = searchIndexEngine.CountDocuments(null).First().Value,
            TotalFilesByExtension = searchIndexEngine.CountDocuments(new CounterRequest
            {
                Field = searchIndexEngine.GetFieldName(x => x.Extension)
            }),
            TotalHiResFiles = searchIndexEngine.Search(new Search.Models.Requests.SearchRequest
            {
                Text = "hr flac",
                SearchFields = new Dictionary<string, string?> { { searchIndexEngine.GetFieldName(x => x.Text), string.Empty } },
                QueryType = QueryTypesEnum.Text,
                Pagination = new PaginationRequest(int.MaxValue, 0)
            }).TotalHits,
            ReleaseYears = searchIndexEngine.CountDocuments(new CounterRequest
            {
                Field = searchIndexEngine.GetFieldName(x => x.Year),
                IsNumeric = true
            }),
            GenreCount = searchIndexEngine.CountDocuments(new CounterRequest
            {
                Field = searchIndexEngine.GetFieldName(x => x.Genre)
            }),
            LatestAdditions = searchIndexEngine.GetLatestAddedItems(new CounterRequest
            {
                Field = searchIndexEngine.GetFieldName(x => x.Release),
                AdditionalField = searchIndexEngine.GetFieldName(x => x.Artist),
                SortByField = searchIndexEngine.GetFieldName(x => x.ModifiedDate),
                IsNumeric = false,
                Top = 25
            })
        };
    }

    private SearchResultDto<MusicLibraryDocument> PerformSearch(
        ISearchIndexEngine<MusicLibraryDocument> searchIndexEngine,
        string query,
        string[]? terms,
        string[] fields,
        QueryTypesEnum queryType,
        PaginationRequest paginationRequest,
        IDictionary<string, IEnumerable<string?>?>? facets = null)
    {
        if (string.IsNullOrEmpty(query) && (terms == null || terms.Length == 0))
            return SearchResultDto<MusicLibraryDocument>.Empty();

        if (fields == null || fields.Length == 0)
            return SearchResultDto<MusicLibraryDocument>.Empty();

        var searchFields = new Dictionary<string, string?>(fields.Length);

        for (var i = 0; i < fields.Length; i++)
            searchFields.Add(fields[i], terms is not null && i < terms.Length ? terms[i] : string.Empty);

        var searchRequest = new Search.Models.Requests.SearchRequest
        {
            Text = query,
            SearchFields = searchFields,
            QueryType = queryType,
            Pagination = paginationRequest,
            Facets = facets,
            SearchType = SearchType.ExactMatch
        };
        var res = searchIndexEngine.Search(searchRequest);
        res.SearchFields = searchFields;

        return res;
    }
}
