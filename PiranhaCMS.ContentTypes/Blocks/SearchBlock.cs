using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Blocks.Base;
using PiranhaCMS.ContentTypes.Constants;
using System.ComponentModel.DataAnnotations;

namespace PiranhaCMS.ContentTypes.Blocks;

[BlockType(
    Name = "Search Block",
    Category = Global.TeasersCategory)]
public class SearchBlock : BlockBase
{
    [Field(
        Title = "Heading",
        Placeholder = "Enter heading text",
        Options = FieldOption.HalfWidth,
        Description = "This is block heading")]
    [Required(ErrorMessage = "Heading: required!")]
    public StringField Heading { get; set; }

    [Field(
        Title = "Lead Text",
        Placeholder = "Enter lead text",
        Options = FieldOption.HalfWidth,
        Description = "This is block lead text")]
    [StringLength(255, ErrorMessage = "Lead text: maximum length is 255 characters!")]
    public TextField LeadText { get; set; }
}
