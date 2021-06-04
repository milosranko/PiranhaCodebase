using Piranha.Extend;
using Piranha.Extend.Fields;

namespace PiranhaCMS.PublicWeb.Models.Regions
{
    public class ArticlePageRegion
    {
        [Field(Title = "Heading", Placeholder = "Heading")]
        [FieldDescription("This is page heading")]
        public StringField Heading { get; set; }

        [Field(Title = "Lead text", Placeholder = "Lead text")]
        [FieldDescription("This is page lead text")]
        public TextField LeadText { get; set; }

        [Field(Title = "Main body", Placeholder = "Main body text")]
        [FieldDescription("This is page main content")]
        public HtmlField MainContent { get; set; }
    }
}
