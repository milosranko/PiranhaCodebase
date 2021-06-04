using Piranha.AttributeBuilder;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages
{
    [PageType(Title = "Search page", UseBlocks = false)]
    [ContentTypeRoute(Title = "Default", Route = "/searchpage")]
    [AllowedPageTypes(Availability.None)]
    public class SearchPage : Page<SearchPage>, IPage
    { }
}
