using Piranha.Extend;
using Piranha.Extend.Fields;

namespace PiranhaCMS.PublicWeb.Models.Regions
{
    public class ArticlePageRegion
    {
        [Field(Title = "Heading", Placeholder = "Heading", Description = "This is page heading")]
        public StringField Heading { get; set; }

        [Field(Title = "Lead text", Placeholder = "Lead text", Description = "This is page lead text")]
        public TextField LeadText { get; set; }

        [Field(Title = "Main body", Placeholder = "Main body text", Description = "This is page main content")]
        public HtmlField MainContent { get; set; }
    }
}
