using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.ContentTypes.Regions;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.ContentTypes.Pages;

[PageType(Title = "Article List Page", UseBlocks = false)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(ArticleListPage)}")]
[AllowedPageTypes(
[
	typeof(ArticlePage)
])]
public class ArticleListPage : Page<ArticleListPage>, IPage
{
	[Region(
		Title = "Main Content",
		Display = RegionDisplayMode.Content,
		Description = "Main content properties")]
	public ArticleListPageRegion PageRegion { get; set; }
}
