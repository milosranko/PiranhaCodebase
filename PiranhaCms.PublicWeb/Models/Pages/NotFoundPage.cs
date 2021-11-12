using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.PublicWeb.Models.Regions;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages
{
    [PageType(Title = "404 Not Found page", UseBlocks = false)]
    [ContentTypeRoute(Title = "Default", Route = "/notfoundpage")]
    [AllowedPageTypes(Availability.None)]
    public class NotFoundPage : Page<NotFoundPage>, IPage
    {
        [Region(
            Title = "Main content",
            Display = RegionDisplayMode.Content,
            Description = "Main content properties")]
        public ArticlePageRegion PageRegion { get; set; }
    }
}
