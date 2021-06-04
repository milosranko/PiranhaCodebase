using PiranhaCMS.PublicWeb.Models.Pages;

namespace PiranhaCMS.PublicWeb.Models.ViewModels
{
    public class NotFoundPageViewModel : PageViewModel<NotFoundPage>
    {
        public NotFoundPageViewModel(NotFoundPage currentPage) : base(currentPage)
        { }
    }
}
