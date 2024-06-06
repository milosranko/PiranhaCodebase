using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Blocks.Base;
using PiranhaCMS.ContentTypes.Constants;
using PiranhaCMS.ContentTypes.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PiranhaCMS.ContentTypes.Blocks;

[BlockType(
    Name = "Teaser Block",
    Category = Global.TeasersCategory)]
public partial class TeaserBlock : BlockBase, ISearchable
{
    [Field(
        Title = "Heading",
        Placeholder = "Enter heading text",
        Description = "This is heading field")]
    [Required(ErrorMessage = "Heading: required!")]
    [StringLength(70, ErrorMessage = $"{nameof(Heading)}: maximum length is 70 characters!")]
    public StringField Heading { get; set; }

    [Field(
        Title = "Lead Text",
        Placeholder = "Enter lead text",
        Description = "This is lead text field")]
    [StringLength(255, ErrorMessage = $"{nameof(LeadText)}: maximum length is 255 characters!")]
    public TextField LeadText { get; set; }

    [Field(
        Title = "Main Text",
        Placeholder = "Enter main text",
        Description = "This is main text field")]
    [StringLength(1000, ErrorMessage = $"{nameof(MainText)}: maximum length is 1000 characters!")]
    public HtmlField MainText { get; set; }

    [Field(
        Title = "Image",
        Placeholder = "Please select image",
        Options = FieldOption.HalfWidth,
        Description = "This is block image")]
    public ImageField Image { get; set; }

    [Field(
        Title = "Image Position",
        Options = FieldOption.HalfWidth,
        Description = "Please select image positioning")]
    public SelectField<ImagePositionEnum> ImagePosition { get; set; }

    #region ISearchable implementation

    public string GetIndexedContent()
    {
        var sb = new StringBuilder();

        sb.AppendLine(Heading.GetIndexedContent());
        sb.AppendLine(LeadText.GetIndexedContent());

        return sb.ToString();
    }

    #endregion
}
