using Piranha;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;
using PiranhaCMS.Search.Models.Enums;
using System.Text;
using static PiranhaCMS.Common.Extensions.StringExtensions;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record MusicSearchPageViewModel : PageViewModel<MusicSearchPage>
{
    public const int PageSize = 20;
    public MusicSearchResult SearchResult { get; private set; }
    public MusicIndexCounts IndexCounts { get; private set; }

    public MusicSearchPageViewModel(
        MusicSearchPage currentPage,
        HttpContext httpContext,
        ICache cache) : base(currentPage)
    {
        SearchResult = MusicSearchResult.Empty;
        IndexCounts = MusicIndexCounts.Empty;

        var musicSearchIndexEngine = httpContext.RequestServices.GetRequiredService<IMusicSearchIndexEngine>();
        var searchText = httpContext.Request.Query["q"].ToString();
        var artist = httpContext.Request.Query["artist"].ToString();
        var release = httpContext.Request.Query["release"].ToString();
        var genre = httpContext.Request.Query["genre"].ToString();
        var year = httpContext.Request.Query["year"].ToString();
        int.TryParse(httpContext.Request.Query["page"], out int pageIndex);
        var paginationQueryString = new StringBuilder();

        if (!string.IsNullOrEmpty(searchText))
        {
            paginationQueryString.Append("?q=");
            paginationQueryString.Append(searchText);

            var searchRequest = new MusicSearchRequest
            {
                Text = searchText.SanitizeSearchString(),
                Fields = [FieldNames.Text],
                QueryType = QueryTypesEnum.Text,
                Pagination = new Pagination(PageSize, pageIndex, paginationQueryString.ToString())
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
        }
        else if (!string.IsNullOrEmpty(release) && !string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append("?artist=");
            paginationQueryString.Append(artist);
            paginationQueryString.Append("&release=");
            paginationQueryString.Append(release);

            var searchRequest = new MusicSearchRequest
            {
                Terms = [artist, release],
                Fields = [FieldNames.Artist, FieldNames.Album],
                QueryType = QueryTypesEnum.MultiTerm,
                Pagination = new Pagination(PageSize, pageIndex, paginationQueryString.ToString())
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
        }
        else if (!string.IsNullOrEmpty(genre))
        {
            paginationQueryString.Append("?genre=");
            paginationQueryString.Append(genre);

            var searchRequest = new MusicSearchRequest
            {
                Text = genre,
                Fields = [FieldNames.Genre],
                QueryType = QueryTypesEnum.Term,
                Pagination = new Pagination(PageSize, pageIndex, paginationQueryString.ToString())
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
        }
        else if (!string.IsNullOrEmpty(year))
        {
            paginationQueryString.Append("?year=");
            paginationQueryString.Append(year);

            var searchRequest = new MusicSearchRequest
            {
                Text = year,
                Fields = [FieldNames.Year],
                QueryType = QueryTypesEnum.Numeric,
                Pagination = new Pagination(PageSize, pageIndex, paginationQueryString.ToString())
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
        }
        else if (!string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append("?artist=");
            paginationQueryString.Append(artist);

            var searchRequest = new MusicSearchRequest
            {
                Text = artist,
                Fields = [FieldNames.Artist],
                QueryType = QueryTypesEnum.Term,
                Pagination = new Pagination(PageSize, pageIndex, paginationQueryString.ToString())
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
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
                var indexCounts = musicSearchIndexEngine.GetIndexStatistics();
                cache.Set(CacheKeys.MusicIndexCount, indexCounts);
                IndexCounts = indexCounts;
            }
        }
    }
}
