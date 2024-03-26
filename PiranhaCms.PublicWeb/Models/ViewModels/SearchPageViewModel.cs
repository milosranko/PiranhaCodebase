using PiranhaCMS.Common.Extensions;
using PiranhaCMS.ContentTypes.Helpers;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Models;
using static PiranhaCMS.Common.Extensions.StringExtensions;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record SearchPageViewModel : PageViewModel<SearchPage>
{
    public SearchResult SearchResult { get; private set; }
    public string? MusicSearchPageName { get; private set; }
    public string? MusicSearchPageLink { get; private set; }

    public SearchPageViewModel(
        SearchPage currentPage,
        HttpRequest httpRequest,
        ISearchIndexEngine engine) : base(currentPage)
    {
        SearchResult = SearchResult.Empty;

        var searchText = httpRequest.Query["q"].ToString();

        if (!string.IsNullOrEmpty(searchText))
        {
            _ = int.TryParse(httpRequest.Query["page"], out int pageIndex);
            var searchRequest = new SearchRequest
            {
                Text = searchText.SanitizeSearchString(),
                Pagination = new Pagination(PageHelpers.GetSiteSettings().PageSize?.Value, pageIndex)
            };

            SearchResult = engine.Search(searchRequest);
        }

        var musicSearchPage = currentPage.GetChildrenPages().AsPage<MusicSearchPage>().FirstOrDefault();
        if (musicSearchPage == null) return;

        MusicSearchPageName = musicSearchPage.Title;
        MusicSearchPageLink = musicSearchPage.Permalink;
    }
}
