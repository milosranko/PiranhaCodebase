using Piranha.Extend;
using Piranha.Runtime;
using System.Reflection;

namespace PiranhaCMS.Common.Extensions;

public static class AppBlockListExtensions
{
	public static void AutoRegisterBlocks(this AppBlockList appBlockList, Assembly blockTypes)
	{
		var method = typeof(AppBlockList).GetMethod(nameof(AppBlockList.Register));
		var types = blockTypes.GetTypes()
			.Where(x => x.GetCustomAttribute<BlockTypeAttribute>() != null || x.GetCustomAttribute<BlockGroupTypeAttribute>() != null)
			.ToArray();
		MethodInfo generic;

		foreach (var type in types)
		{
			generic = method.MakeGenericMethod(type);
			generic.Invoke(appBlockList, null);
		}
	}
}
