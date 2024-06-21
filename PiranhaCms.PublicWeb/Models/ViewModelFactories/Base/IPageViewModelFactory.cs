using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.PublicWeb.Models.ViewModels.Base;

namespace PiranhaCMS.PublicWeb.Models.ViewModelFactories.Base;

public interface IPageViewModelFactory<TPage, TViewModel>
    where TPage : IPage
    where TViewModel : PageViewModel<TPage>
{
    TViewModel Create(TPage page);
}
