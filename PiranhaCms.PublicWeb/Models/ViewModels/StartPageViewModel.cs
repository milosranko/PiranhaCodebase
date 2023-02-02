using PiranhaCMS.PublicWeb.Models.Pages;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public class StartPageViewModel : PageViewModel<StartPage>
{
    public StartPageViewModel(StartPage currentPage) : base(currentPage)
    { }
}
