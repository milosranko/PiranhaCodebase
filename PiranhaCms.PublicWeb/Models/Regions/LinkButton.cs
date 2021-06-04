using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System.ComponentModel.DataAnnotations;

namespace PiranhaCMS.PublicWeb.Models.Regions
{
    public class LinkButton
    {
        [Field(Title = "Link text", Placeholder = "Enter link text")]
        [FieldDescription("Link text")]
        [Required(ErrorMessage = "Link text: required!")]
        public StringField LinkName { get; set; }

        [Field(Title = "Page reference", Options = FieldOption.HalfWidth)]
        [FieldDescription("Please select page reference")]
        public PageField Page { get; set; }

        [Field(Title = "External URL address", Options = FieldOption.HalfWidth)]
        [FieldDescription("Enter full external URL (including http...)")]
        public StringField ExternalUrl { get; set; }

        [Field(Title = "Open link in new window", Options = FieldOption.HalfWidth)]
        [FieldDescription("Check if you want link to be opened in a new window/tab")]
        public CheckBoxField OpenInNewWindow { get; set; }
    }
}
