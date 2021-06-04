using Microsoft.AspNetCore.Http;
using PiranhaCMS.PublicWeb.Helpers;
using PiranhaCMS.PublicWeb.Models.Pages;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Models;

namespace PiranhaCMS.PublicWeb.Models.ViewModels
{
    public class SearchPageViewModel : PageViewModel<SearchPage>
    {
        public const int PageSizeFallback = 5;
        public SearchResult SearchResult { get; set; }

        public SearchPageViewModel(
            SearchPage currentPage,
            HttpRequest httpRequest,
            ISearchIndexEngine searchIndexEngine) : base(currentPage)
        {
            SearchResult = SearchResult.Empty();

            var searchText = httpRequest.Query["q"];

            if (!string.IsNullOrEmpty(searchText))
            {
                int.TryParse(httpRequest.Query["page"], out int pageIndex);
                var searchRequest = new SearchRequest
                {
                    Text = searchText,
                    Pagination = new Pagination(PageHelpers.GetSiteSettings().PageSize?.Value ?? PageSizeFallback, pageIndex)
                };

                SearchResult = searchIndexEngine.Search(searchRequest);
            }
        }
    }
}
