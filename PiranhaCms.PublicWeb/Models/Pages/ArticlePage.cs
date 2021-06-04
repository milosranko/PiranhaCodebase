using PiranhaCMS.PublicWeb.Models.Pages.Base;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.Validators.Attributes;
using PiranhaCMS.PublicWeb.Models.Regions;

namespace PiranhaCMS.PublicWeb.Models.Pages
{
    [PageType(Title = "Article page", UseBlocks = true)]
    [ContentTypeRoute(Title = "Default", Route = "/articlepage")]
    [AllowedPageTypes(new []
    {
        typeof(ArticlePage)
    })]
    public class ArticlePage : Page<ArticlePage>, IPage
    {
        [Region(
            Title = "Main content", 
            Display = RegionDisplayMode.Content)]
        [RegionDescription("Main content properties")]
        public ArticlePageRegion PageRegion { get; set; }
    }
}
