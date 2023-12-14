using Piranha.Models;
using PiranhaCMS.Common.Helpers;

namespace PiranhaCMS.Common.Extensions;

public static class EnumerableExtensions
{
	public static IEnumerable<T> AsPage<T>(this IEnumerable<SitemapItem> source) where T : PageBase
	{
		if (source == null || !source.Any())
			yield break;

		foreach (var item in source.Select(x => PageHelpers.GetPageById<T>(x.Id)))
			yield return item;
	}
}
