using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages;
using PiranhaCMS.PublicWeb.Models.Regions;
using PiranhaCMS.Validators.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PiranhaCMS.PublicWeb.Models.Sites
{
    [SiteType(Title = "Public Site")]
    [AllowedPageTypes(new[]
    {
        typeof(StartPage),
        typeof(ArticlePage),
        typeof(SearchPage),
        typeof(NotFoundPage)
    })]
    public class PublicSite : SiteContent<PublicSite>
    {
        [Region(
            Title = "Site Footer",
            Display = RegionDisplayMode.Setting,
            Description = "Site footer properties")]
        public SiteFooter SiteFooter { get; set; }

        [Region(
            Title = "Global Settings",
            Display = RegionDisplayMode.Setting,
            Description = "Global settings")]
        public GlobalSettings GlobalSettings { get; set; }

        [Region(
            Title = "Top Links",
            Display = RegionDisplayMode.Content,
            Description = "Top links")]
        public IList<LinkButton> TopLinks { get; set; }
    }

    public class SiteFooter
    {
        [Field(
            Title = "Column 1 Header",
            Description = "Column 1 header")]
        [Required(ErrorMessage = "Column 1 Header: required!")]
        public StringField Column1Header { get; set; }

        [Field(
            Title = "Column 1 Content",
            Description = "Column 1 content")]
        public HtmlField Column1Content { get; set; }

        [Field(
            Title = "Column 2 Header",
            Description = "Column 2 header")]
        [Required(ErrorMessage = "Column 2 Header: required!")]
        public StringField Column2Header { get; set; }

        [Field(
            Title = "Column 2 Content",
            Description = "Column 2 content")]
        public HtmlField Column2Content { get; set; }

        [Field(
            Title = "Column 3 Header",
            Description = "Column 3 header")]
        [Required(ErrorMessage = "Column 3 Header: required!")]
        public StringField Column3Header { get; set; }

        [Field(
            Title = "Column 3 Content",
            Description = "Column 3 content")]
        public HtmlField Column3Content { get; set; }

        [Field(
            Title = "Column 4 Header",
            Description = "Column 4 header")]
        [Required(ErrorMessage = "Column 4 Header: required!")]
        public StringField Column4Header { get; set; }

        [Field(
            Title = "Column 4 Content",
            Description = "Column 4 content")]
        public HtmlField Column4Content { get; set; }
    }

    public class GlobalSettings
    {
        [Field(
            Title = "Contact E-mail Address",
            Description = "E-mail address")]
        public StringField EmailAddress { get; set; }

        [Field(
            Title = "Contact Phone Number",
            Description = "Phone number")]
        public StringField PhoneNumber { get; set; }

        [Field(
            Title = "Search Page Reference",
            Description = "Reference to search page")]
        public PageField SearchPageReference { get; set; }

        [Field(
            Title = "Page Size",
            Description = "Number of search results per page")]
        public NumberField PageSize { get; set; }
    }
}
