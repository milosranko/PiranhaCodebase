using Piranha.Extend;
using Piranha.Extend.Fields;

namespace PiranhaCMS.ContentTypes.Regions;

public class ArticleListPageRegion
{
    [Field(
        Title = "Heading",
        Placeholder = "Heading",
        Description = "This is page heading")]
    public StringField Heading { get; set; }

    [Field(
        Title = "Lead Text",
        Placeholder = "Lead text",
        Description = "This is page lead text")]
    public TextField LeadText { get; set; }
}
