using PiranhaCMS.PublicWeb.Models.Pages.Base;

namespace PiranhaCMS.PublicWeb.Models.ViewModels
{
    public interface IPageViewModel<out T> where T : IPage
    {
        T CurrentPage { get; }
        HeaderViewModel Header { get; set; }
        FooterViewModel Footer { get; set; }
        GlobalSettingsViewModel GlobalSettings { get; set; }
    }
}
