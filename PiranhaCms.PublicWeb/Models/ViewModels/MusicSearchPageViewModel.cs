using Piranha;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;
using PiranhaCMS.Search.Models.Enums;
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
        int.TryParse(httpContext.Request.Query["page"], out int pageIndex);

        if (!string.IsNullOrEmpty(searchText))
        {
            var searchRequest = new MusicSearchRequest
            {
                Text = searchText.SanitizeSearchString(),
                Fields = [FieldNames.Text],
                QueryType = QueryTypesEnum.Text,
                Pagination = new Pagination(PageSize, pageIndex)
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
        }
        else if (!string.IsNullOrEmpty(artist))
        {
            var searchRequest = new MusicSearchRequest
            {
                Text = artist.SanitizeSearchString(),
                Fields = [FieldNames.Artist],
                QueryType = QueryTypesEnum.Term,
                Pagination = new Pagination(PageSize, pageIndex)
            };

            SearchResult = musicSearchIndexEngine.Search(searchRequest);
        }
        else if (!string.IsNullOrEmpty(release))
        {
            var searchRequest = new MusicSearchRequest
            {
                Text = release.SanitizeSearchString(),
                Fields = [FieldNames.Album],
                QueryType = QueryTypesEnum.Term,
                Pagination = new Pagination(PageSize, pageIndex)
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
