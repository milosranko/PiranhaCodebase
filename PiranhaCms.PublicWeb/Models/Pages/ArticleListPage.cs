using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.PublicWeb.Models.Regions;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages;

[PageType(Title = "Article List Page", UseBlocks = false)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(ArticleListPage)}")]
[AllowedPageTypes(new[]
{
    typeof(ArticlePage)
})]
public class ArticleListPage : Page<ArticleListPage>, IPage
{
    [Region(
        Title = "Main Content",
        Display = RegionDisplayMode.Content,
        Description = "Main content properties")]
    public ArticleListPageRegion PageRegion { get; set; }
}
