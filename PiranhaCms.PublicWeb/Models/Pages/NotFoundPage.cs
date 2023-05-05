using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.PublicWeb.Models.Regions;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages;

[PageType(Title = "404 Not Found Page", UseBlocks = false, UsePrimaryImage = false, UseExcerpt = false)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(NotFoundPage)}")]
[AllowedPageTypes(Availability.None)]
public class NotFoundPage : Page<NotFoundPage>, IPage
{
    [Region(
        Title = "Main Content",
        Display = RegionDisplayMode.Content,
        Description = "Main content properties")]
    public ArticlePageRegion PageRegion { get; set; }
}
