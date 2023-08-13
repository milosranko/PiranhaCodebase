using PiranhaCMS.ContentTypes.Pages.Base;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public class PageViewModel<T> : IPageViewModel<T> where T : IPage
{
    public PageViewModel(T currentPage)
    {
        CurrentPage = currentPage;
    }

    public T CurrentPage { get; }
    public HeaderViewModel Header { get; set; }
    public FooterViewModel Footer { get; set; }
    public GlobalSettingsViewModel GlobalSettings { get; set; }
}
