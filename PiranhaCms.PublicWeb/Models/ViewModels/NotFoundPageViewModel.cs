using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.PublicWeb.Models.ViewModels.Base;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record NotFoundPageViewModel : PageViewModel<NotFoundPage>
{
	public NotFoundPageViewModel(NotFoundPage currentPage) : base(currentPage)
	{ }
}
