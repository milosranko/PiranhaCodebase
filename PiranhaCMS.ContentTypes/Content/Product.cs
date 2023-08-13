using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace PiranhaCMS.ContentTypes.Content;

[ContentGroup(Title = "Products", Icon = "fas fa-hammer")]
[ContentType(Title = "Product")]
public class Product : Content<Product>, ICategorizedContent, ITaggedContent
{
    public Taxonomy Category { get; set; }

    public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

    [Region(
        Title = "Product Properties",
        Display = RegionDisplayMode.Content,
        Description = "Main product properties")]
    public ProductRegion ProductProperties { get; set; }
}

public class ProductRegion
{
    [Field(
        Title = "Name",
        Placeholder = "Name",
        Description = "Product name")]
    public StringField Name { get; set; }

    [Field(
        Title = "Description",
        Placeholder = "Description",
        Description = "Product description")]
    public HtmlField Description { get; set; }

    [Field(
        Title = "Price",
        Placeholder = "Price",
        Description = "Product price")]
    public NumberField Price { get; set; }
}
