using PiranhaCMS.ContentTypes.Pages;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record NotFoundPageViewModel : PageViewModel<NotFoundPage>
{
	public NotFoundPageViewModel(NotFoundPage currentPage) : base(currentPage)
	{ }
}
