using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Dto;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record MusicSearchPageViewModel : PageViewModel<MusicSearchPage>
{
    public const int PageSize = 20;
    public SearchResultDto<MusicLibraryDocument> SearchResult { get; set; }
    public MusicIndexCounts IndexCounts { get; set; }

    public MusicSearchPageViewModel(MusicSearchPage currentPage) : base(currentPage)
    {
        SearchResult = SearchResultDto<MusicLibraryDocument>.Empty();
        IndexCounts = MusicIndexCounts.Empty;
    }
}
