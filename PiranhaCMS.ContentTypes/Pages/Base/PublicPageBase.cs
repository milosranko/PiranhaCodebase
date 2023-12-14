using Piranha.Models;

namespace PiranhaCMS.ContentTypes.Pages.Base;

public abstract class PublicPageBase<T> : Page<T> where T : Page<T>
{
	//public bool IsStartPage => !ParentId.HasValue && SortOrder == 0;
}
