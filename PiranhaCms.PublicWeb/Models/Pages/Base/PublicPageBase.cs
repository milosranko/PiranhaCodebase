using Piranha.Models;

namespace Demo_PiranhaCMS.Models.Pages.Base
{
    public abstract class PublicPageBase<T> : Page<T> where T : Page<T>
    {
        //public bool IsStartPage => !ParentId.HasValue && SortOrder == 0;
    }
}
