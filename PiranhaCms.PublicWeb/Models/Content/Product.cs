using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System.Collections.Generic;

namespace PiranhaCMS.PublicWeb.Models.Content
{
    [ContentGroup(Title = "Products", Icon = "fas fa-hammer")]
    [ContentType(Title = "Product")]
    public class Product : Content<Product>, ICategorizedContent, ITaggedContent
    {
        public Taxonomy Category { get; set; }

        public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

        [Region(
            Title = "Product properties",
            Display = RegionDisplayMode.Content)]
        [RegionDescription("Main product properties")]
        public ProductRegion ProductProperties { get; set; }
    }

    public class ProductRegion
    {
        [Field(Title = "Name", Placeholder = "Name")]
        [FieldDescription("Product name")]
        public StringField Name { get; set; }

        [Field(Title = "Description", Placeholder = "Description")]
        [FieldDescription("Product description")]
        public HtmlField Description { get; set; }

        [Field(Title = "Price", Placeholder = "Price")]
        [FieldDescription("Product price")]
        public NumberField Price { get; set; }
    }
}
