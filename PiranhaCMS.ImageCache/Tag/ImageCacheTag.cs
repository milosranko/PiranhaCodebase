using Microsoft.AspNetCore.Razor.TagHelpers;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Fields;

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

            SetImageAttributes(imageField, output.Attributes, out var resizeParams);

            //TODO Check if image sizes exists, process images resizing if needed
            if (resizeParams.Length == 0) return;

            Parallel.ForEach(resizeParams, x => _appService.Media.ResizeImage(imageField, x.w, x.h));
        }
    }

    private void SetImageAttributes(ImageField imageRef, TagHelperAttributeList attributes, out ResizeParams[] resizeParams)
    {
        var media = imageRef.Media;
        var imageAlt = media.AltText ?? string.Empty;
        var imageUrl = media.PublicUrl.StartsWith("~")
            ? media.PublicUrl.Substring(1)
            : media.PublicUrl;
        var src = imageUrl;

        if (!string.IsNullOrEmpty(SrcSet))
        {
            src = imageUrl.GetSrc(SrcSet.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).First());
            attributes.Add("srcset", imageUrl.GetSrcSet(out resizeParams, SrcSet));
        }
        else
        {
            resizeParams = [];
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
