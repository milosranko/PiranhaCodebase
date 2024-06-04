using System.Text;

namespace PiranhaCMS.ImageCache.Tag;

internal static class ImageCacheTagHelpers
{
    public static string GetSrc(this string imageUrl, string srcSet = null)
    {
        if (srcSet == null)
            return imageUrl;

        var dim = srcSet.Split('x');
        var width = dim[0];
        var height = string.Empty;

        if (dim.Length == 2)
        {
            height = dim[1];
        }

        return CreateNewFileUrl(imageUrl, width, height);
    }

    public static string GetSrcSet(this string imageUrl, out ResizeParams[] resizeParams, string srcSet = null)
    {
        if (srcSet == null)
        {
            resizeParams = [];
            return imageUrl;
        }

        var breakingPoints = srcSet.Split('|');
        var result = new StringBuilder();
        var index = 0;
        resizeParams = new ResizeParams[breakingPoints.Length];

        foreach (var point in breakingPoints)
        {
            var dim = point.Split('x');
            var width = dim[0];
            var height = string.Empty;

            if (dim.Length == 2)
            {
                height = dim[1];
            }

            resizeParams[index] = new ResizeParams
            {
                w = int.Parse(width),
                h = string.IsNullOrEmpty(height) ? default : int.Parse(height)
            };
            result.Append($"{CreateNewFileUrl(imageUrl, width, height)} {width}w, ");
            index++;
        }

        return result.ToString().TrimEnd(' ', ',');
    }

    public static string CreateNewFileUrl(string imageUrl, string width, string height)
    {
        var newFileName = $"{Path.GetFileNameWithoutExtension(imageUrl)}_{width}x{height}";
        return $"{imageUrl.Replace(Path.GetFileNameWithoutExtension(imageUrl), newFileName)}";
    }
}
