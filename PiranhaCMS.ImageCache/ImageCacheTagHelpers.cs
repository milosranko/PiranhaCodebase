using System.Text;

namespace PiranhaCMS.ImageCache
{
    internal static class ImageCacheTagHelpers
    {
        public static string GetSrc(this string imageUrl, string srcSet = null)
        {
            var result = new StringBuilder();

            if (srcSet == null)
                return imageUrl;

            var dim = srcSet.Split('x');
            var width = dim[0];
            var height = string.Empty;

            if (dim.Length == 2)
            {
                height = "&h=" + dim[1];
            }

            return result
                .AppendFormat($"{imageUrl}?w={width}{height}")
                .ToString();
        }

        public static string GetSrcSet(this string imageUrl, string srcSet = null)
        {
            if (srcSet == null)
                return imageUrl;

            var breakingPoints = srcSet.Split('|');
            var result = new StringBuilder();

            foreach (var point in breakingPoints)
            {
                var dim = point.Split('x');
                var width = dim[0];
                var height = string.Empty;

                if (dim.Length == 2)
                {
                    height = "&h=" + dim[1];
                }

                result.AppendFormat($"{imageUrl}?w={width}{height} {width}w, ");
            }

            return result.ToString().TrimEnd(' ', ',');
        }
    }
}
