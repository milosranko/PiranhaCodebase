using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.PublicWeb.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages
{
    [PageType(Title = "Start page", UseBlocks = true)]
    [ContentTypeRoute(Title = "Default", Route = "/startpage")]
    [AllowedPageTypes(Availability.None)]
    public class StartPage : Page<StartPage>, IPage
    {
        [Region(
            Title = "Header", 
            Display = RegionDisplayMode.Content,
            Description = "Page header properties")]
        public PageHeaderRegion Header { get; set; }

        [Region(
            Title = "Top links", 
            Display = RegionDisplayMode.Content,
            Description = "Top links")]
        public IList<LinkButton> TopLinks { get; set; }

        public class PageHeaderRegion
        {
            [Field(Title = "Heading", Placeholder = "Heading", Description = "This is page heading")]
            [Required(ErrorMessage = "Heading: required!")]
            [StringLength(15, ErrorMessage = "Heading: maximum allowed characters is 15!")]
            public StringField Heading { get; set; }

            [Field(Title = "Top image", Description = "This top image selector")]
            [Required(ErrorMessage = "Top image: required!")]
            public ImageField TopImage { get; set; }
        }
    }
}
