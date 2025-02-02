using Microsoft.AspNetCore.Razor.TagHelpers;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Fields;
using PiranhaCMS.ImageCache.Models;
using PiranhaCMS.ImageCache.Services;
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
    public required object Model { get; set; }

    [HtmlAttributeName("mode")]
    public ResizeMode Mode { get; set; } = ResizeMode.Undefined;

    [HtmlAttributeName("altfallback")]
    public string? AltFallback { get; set; }

    private readonly IApplicationService _appService;
    private readonly IImageCacheService _imageCacheService;

    public ImageCacheTag(IApplicationService appService, IImageCacheService imageCacheService)
    {
        _appService = appService;
        _imageCacheService = imageCacheService;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Model == null)
        {
            output.Content.Clear();
            return;
        }

        //TODO Add converting of existing JPG, JPEG, PNG, TIF, TIFF images to WebP

        if (Model.GetType() == typeof(ImageField))
        {
            var imageField = (ImageField)Model;

            if (!imageField.HasValue) return;

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;

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
            var breakingPoints = SrcSet.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
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

        attributes.Add("loading", "lazy");
        attributes.Add("decoding", "async");
    }

    private string GetSrc(ImageField image, string dimensions)
    {
        var dim = dimensions.Split('x');
        var width = dim[0];
        var height = string.Empty;

        if (dim.Length == 2)
            height = dim[1];

        return SanitizeImageUrl(_appService.Media.ResizeImage(
            image,
            int.Parse(width),
            string.IsNullOrEmpty(height) ? default : int.Parse(height)));
    }

    private string GetSrcSet(ImageField image, string[] breakingPoints)
    {
        var result = new StringBuilder();
        var mediaId = Guid.Empty;
        var media = image.Media;

        if (!Path.GetExtension(image.Media.Filename).Equals(StringConstants.WEBP_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
            _imageCacheService.ConvertToWebP(image.Media, out mediaId);

        if (mediaId != Guid.Empty)
            media = _imageCacheService.GetById(mediaId);

        foreach (var point in breakingPoints)
        {
            var dim = point.Split('x');
            var width = dim[0];
            var height = string.Empty;

            if (dim.Length == 2)
                height = dim[1];

            var imageVariantUrl = _appService.Media.ResizeImage(
                media,
                int.Parse(width),
                string.IsNullOrEmpty(height) ? default : int.Parse(height));

            result.Append($"{SanitizeImageUrl(imageVariantUrl)} {width}w, ");
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
