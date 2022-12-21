using Piranha.AttributeBuilder;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages
{
    [PageType(Title = "Search Page", UseBlocks = false)]
    [ContentTypeRoute(Title = "Default", Route = $"/{nameof(SearchPage)}")]
    [AllowedPageTypes(Availability.None)]
    public class SearchPage : Page<SearchPage>, IPage
    { }
}
