using Piranha.Models;
using PiranhaCMS.Common.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace PiranhaCMS.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsPage<T>(this IEnumerable<SitemapItem> source) where T : PageBase
        {
            if (source == null || !source.Any())
                return Enumerable.Empty<T>();

            var pages = source.Select(x => PageHelpers.GetPageById<T>(x.Id));

            if (pages != null && pages.Any())
                return pages;
            else
                return Enumerable.Empty<T>();
        }
    }
}
