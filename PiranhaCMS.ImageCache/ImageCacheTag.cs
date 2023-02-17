using Microsoft.AspNetCore.Razor.TagHelpers;
using Piranha.Extend.Fields;

namespace PiranhaCMS.ImageCache;

[HtmlTargetElement("image-cache")]
public class ImageCacheTag : TagHelper
{
    [HtmlAttributeName("css")]
    public string? Css { get; set; }

    [HtmlAttributeName("style")]
    public string? Style { get; set; }

    [HtmlAttributeName("srcset")]
    public string? SrcSet { get; set; }

    [HtmlAttributeName("sizes")]
    public string? Sizes { get; set; }

    [HtmlAttributeName("model")]
    public object Model { get; set; }

    [HtmlAttributeName("altfallback")]
    public string? AltFallback { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Model == null)
        {
            output.Content.Clear();
            return;
        }

        if (Model.GetType() == typeof(ImageField) && ((ImageField)Model).HasValue)
        {
            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;

            SetImageAttributes((ImageField)Model, output.Attributes);
        }
    }

    private void SetImageAttributes(ImageField imageRef, TagHelperAttributeList attributes)
    {
        var media = imageRef.Media;
        var imageAlt = media.AltText ?? string.Empty;
        var imageUrl = media.PublicUrl.Remove(0, 1);
        var src = imageUrl;

        if (!string.IsNullOrEmpty(SrcSet))
        {
            src = imageUrl.GetSrc(SrcSet.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).First());

            if (!string.IsNullOrEmpty(Sizes))
            {
                attributes.Add("srcset", imageUrl.GetSrcSet(SrcSet));
            }
        }

        attributes.Add("src", src);

        if (!string.IsNullOrEmpty(imageAlt))
        {
            attributes.Add("alt", imageAlt);
        }
        else if (!string.IsNullOrEmpty(AltFallback))
        {
            attributes.Add("alt", AltFallback);
        }

        if (!string.IsNullOrEmpty(Css))
        {
            attributes.Add("class", Css);
        }

        if (!string.IsNullOrEmpty(Style))
        {
            attributes.Add("style", Style);
        }

        if (!string.IsNullOrEmpty(Sizes))
        {
            attributes.Add("sizes", Sizes);
        }
    }
}
