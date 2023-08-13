using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Blocks.Base;
using PiranhaCMS.ContentTypes.Constants;
using System.ComponentModel.DataAnnotations;

namespace PiranhaCMS.ContentTypes.Blocks;

[BlockType(
    Name = "Carousel Item Block",
    Category = Global.CarouselCategory)]
public class CarouselItemBlock : BlockBase
{
    [Field(
        Title = "Title",
        Placeholder = "Enter title text",
        Description = "This is title field")]
    [StringLength(50, ErrorMessage = "Title text: maximum length is 50 characters!")]
    public StringField Title { get; set; }

    [Field(
        Title = "Heading",
        Placeholder = "Enter heading text",
        Description = "This is heading field")]
    [Required(ErrorMessage = "Heading: required!")]
    [StringLength(50, ErrorMessage = "Heading text: maximum length is 50 characters!")]
    public StringField Heading { get; set; }

    [Field(
        Title = "Background Image",
        Placeholder = "Please select image",
        Options = FieldOption.HalfWidth,
        Description = "This is image field")]
    public ImageField Image { get; set; }

    [Field(
        Title = "Image Alternate Text",
        Placeholder = "Enter Image alternate text",
        Options = FieldOption.HalfWidth,
        Description = "This is image alternate text field")]
    [StringLength(50, ErrorMessage = "Image Alternate Text: maximum length is 50 characters!")]
    public StringField ImageAltText { get; set; }

    [Field(
        Title = "Left Button Text",
        Placeholder = "Enter left button text",
        Options = FieldOption.HalfWidth,
        Description = "This is left button text field")]
    [StringLength(50, ErrorMessage = "Left Button Text: maximum length is 50 characters!")]
    public StringField LeftButtonText { get; set; }

    [Field(
        Title = "Left Button Page Reference",
        Options = FieldOption.HalfWidth,
        Description = "Please select page reference for left button")]
    public PageField LeftButtonPage { get; set; }

    [Field(
        Title = "Right Button Text",
        Placeholder = "Enter right button text",
        Options = FieldOption.HalfWidth,
        Description = "This is right button text field")]
    [StringLength(50, ErrorMessage = "Right Button Text: maximum length is 50 characters!")]
    public StringField RightButtonText { get; set; }

    [Field(
        Title = "Right Button Page Reference",
        Options = FieldOption.HalfWidth,
        Description = "Please select page reference for right button")]
    public PageField RightButtonPage { get; set; }
}
