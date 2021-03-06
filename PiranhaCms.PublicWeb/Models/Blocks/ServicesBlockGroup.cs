using PiranhaCMS.PublicWeb.Business.Constants;
using PiranhaCMS.PublicWeb.Models.Blocks.Base;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PiranhaCMS.PublicWeb.Models.Blocks
{
    [BlockGroupType(
        Name = "Services block", 
        Display = BlockDisplayMode.Vertical, 
        Category = Global.TeasersCategory)]
    [BlockItemType(Type = typeof(TeaserBlock))]
    public class ServicesBlockGroup : BlockGroupBase, ISearchable
    {
        [Field(Title = "Title")]
        [FieldDescription("This is block main heading")]
        [Required(ErrorMessage = "Title: required!")]
        public StringField Title { get; set; }

        [Field(Title = "Heading")]
        [FieldDescription("This is block sub heading")]
        [Required(ErrorMessage = "Heading: required!")]
        public StringField Heading { get; set; }

        #region ISearchable implementation

        public string GetIndexedContent()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Heading.GetIndexedContent());
            sb.AppendLine(Title.GetIndexedContent());

            return sb.ToString();
        }

        #endregion
    }
}
