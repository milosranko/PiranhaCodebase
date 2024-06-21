using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.PublicWeb.Models.ViewModels.Base;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record StartPageViewModel : PageViewModel<StartPage>
{
    public StartPageViewModel(StartPage currentPage) : base(currentPage)
    { }
}
