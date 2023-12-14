using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.ContentTypes.Regions;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.ContentTypes.Pages;

[PageType(Title = "Article Page", UseBlocks = true)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(ArticlePage)}")]
[AllowedPageTypes(
[
	typeof(ArticlePage)
])]
public class ArticlePage : Page<ArticlePage>, IPage
{
	[Region(
		Title = "Main Content",
		Display = RegionDisplayMode.Content,
		Description = "Main content properties")]
	public ArticlePageRegion PageRegion { get; set; }
}
