using PiranhaCMS.ContentTypes.Pages;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record StartPageViewModel : PageViewModel<StartPage>
{
	public StartPageViewModel(StartPage currentPage) : base(currentPage)
	{ }
}
