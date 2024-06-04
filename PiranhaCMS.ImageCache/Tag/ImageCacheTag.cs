using Microsoft.AspNetCore.Razor.TagHelpers;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Fields;
using System.Text;

namespace PiranhaCMS.ImageCache.Tag;

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

    [HtmlAttributeName("mode")]
    public ResizeMode Mode { get; set; } = ResizeMode.Undefined;

    [HtmlAttributeName("altfallback")]
    public string? AltFallback { get; set; }

    private readonly IApplicationService _appService;

    public ImageCacheTag(IApplicationService appService)
    {
        _appService = appService;
    }

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
            var imageField = (ImageField)Model;

            SetImageAttributes(imageField, output.Attributes);
        }
    }

    private void SetImageAttributes(ImageField imageRef, TagHelperAttributeList attributes)
    {
        var media = imageRef.Media;
        var imageAlt = media.AltText ?? string.Empty;
        var imageUrl = SanitizeImageUrl(media.PublicUrl);
        var src = imageUrl;

        if (!string.IsNullOrEmpty(SrcSet))
        {
            var breakingPoints = SrcSet.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            src = GetSrc(imageRef, breakingPoints[0]);
            attributes.Add("srcset", GetSrcSet(imageRef, breakingPoints));
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

    private string GetSrc(ImageField image, string dimensions)
    {
        var dim = dimensions.Split('x');
        var width = dim[0];
        var height = string.Empty;

        if (dim.Length == 2)
            height = dim[1];

        return _appService.Media.ResizeImage(image, int.Parse(width), string.IsNullOrEmpty(height) ? default : int.Parse(height));
    }

    private string GetSrcSet(ImageField image, string[] breakingPoints)
    {
        var result = new StringBuilder();

        foreach (var point in breakingPoints)
        {
            var dim = point.Split('x');
            var width = dim[0];
            var height = string.Empty;

            if (dim.Length == 2)
                height = dim[1];

            result.Append($"{SanitizeImageUrl(_appService.Media.ResizeImage(image, int.Parse(width), string.IsNullOrEmpty(height) ? default : int.Parse(height)))} {width}w, ");
        }

        return result.ToString().TrimEnd(' ', ',');
    }

    private string SanitizeImageUrl(string imageUrl)
    {
        return imageUrl.StartsWith("~")
            ? imageUrl.Substring(1)
            : imageUrl;
    }
}
