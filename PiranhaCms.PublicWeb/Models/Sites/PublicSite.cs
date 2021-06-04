using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages;
using PiranhaCMS.Validators.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PiranhaCMS.PublicWeb.Models.Sites
{
    [SiteType(Title = "Public site")]
    [AllowedPageTypes(new [] 
    {
        typeof(StartPage),
        typeof(ArticlePage),
        typeof(SearchPage),
        typeof(NotFoundPage)
    })]
    public class PublicSite : SiteContent<PublicSite>
    {
        [Region(Title = "Site header", Display = RegionDisplayMode.Content)]
        [RegionDescription("Site header properties")]
        public SiteHeader SiteHeader { get; set; }

        [Region(Title = "Site footer", Display = RegionDisplayMode.Setting)]
        [RegionDescription("Site footer properties")]
        public SiteFooter SiteFooter { get; set; }

        [Region(Title = "Global settings", Display = RegionDisplayMode.Setting)]
        [RegionDescription("Global settings")]
        public GlobalSettings GlobalSettings { get; set; }
    }

    public class SiteFooter
    {
        [Field(Title = "Left column")]
        [FieldDescription("Footer left column")]
        public HtmlField FooterColumn1 { get; set; }

        [Field(Title = "Right column")]
        [FieldDescription("Footer right column")]
        public HtmlField FooterColumn2 { get; set; }
    }

    public class SiteHeader
    {
        [Field(Title = "Site name", Options = FieldOption.HalfWidth)]
        [FieldDescription("Site name visible in page title")]
        [Required(ErrorMessage = "Site name: required!")]
        public StringField SiteName { get; set; }

        [Field(Title = "Logo", Options = FieldOption.HalfWidth)]
        [FieldDescription("Company logo, supported file format: SVG")]
        public DocumentField LogoImage { get; set; }
    }

    public class GlobalSettings
    {
        [Field(Title = "Search page reference")]
        [FieldDescription("Reference to search page")]
        public PageField SearchPageReference { get; set; }

        [Field(Title = "Page size")]
        [FieldDescription("Number of search results per page")]
        public NumberField PageSize { get; set; }
    }
}
