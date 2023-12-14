using Piranha;
using Piranha.Extend.Fields;
using System.Text;

namespace PiranhaCMS.Common.Extensions;

public static class ImageFieldExtensions
{
	public static string GetSrcSet(this ImageField image, params int[] sizes)
	{
		if (image == null || sizes.Length == 0) return string.Empty;

		using var serviceScope = ServiceActivator.GetScope();
		var api = (IApi)serviceScope.ServiceProvider.GetService(typeof(IApi));
		var sb = new StringBuilder();

		foreach (var size in sizes)
		{
			sb.Append(image.Resize(api, size).Remove(0, 1));
			sb.Append($" {size}w");

			if (!(sizes.Last() == size))
			{
				sb.Append(",");
			}
		}

		return sb.ToString();
	}

	public static string GetSrc(this ImageField image, int size)
	{
		if (image == null) return string.Empty;

		using var serviceScope = ServiceActivator.GetScope();
		var api = (IApi)serviceScope.ServiceProvider.GetService(typeof(IApi));

		return image.Resize(api, size).Remove(0, 1);
	}
}
