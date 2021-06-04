using PiranhaCMS.PublicWeb.Business.Constants;
using PiranhaCMS.PublicWeb.Models.Blocks.Base;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PiranhaCMS.PublicWeb.Models.Blocks
{
    [BlockType(
        Name = "Teaser block", 
        Category = Global.TeasersCategory)]
    public class TeaserBlock : BlockBase, ISearchable
    {
        [Field(Title = "Heading", Placeholder = "Enter heading text")]
        [FieldDescription("This is block heading")]
        [Required(ErrorMessage = "Heading: required!")]
        public StringField Heading { get; set; }
        
        [Field(Title = "Lead text", Placeholder = "Enter lead text")]
        [FieldDescription("This is block lead text")]
        [StringLength(50, ErrorMessage = "Lead text: maximum length is 50 characters!")]
        public TextField LeadText { get; set; }
        
        [Field(Title = "Image", Placeholder = "Please select image", Options = FieldOption.HalfWidth)]
        [FieldDescription("This is block image")]
        public ImageField Image { get; set; }
        
        [Field(Title = "Image position", Options = FieldOption.HalfWidth)]
        [FieldDescription("Please select image positioning")]
        public SelectField<ImagePositionEnum> ImagePosition { get; set; }

        public enum ImagePositionEnum
        {
            Left,
            Right
        }

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
}
