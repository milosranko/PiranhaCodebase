using Microsoft.Extensions.DependencyInjection;

namespace PiranhaCMS.Common;

public class ServiceActivator
{
	internal static IServiceProvider? _serviceProvider = null;

	public static void Configure(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public static IServiceScope GetScope(IServiceProvider? serviceProvider = null)
	{
		if (serviceProvider is null && _serviceProvider is null)
			throw new ArgumentException(nameof(serviceProvider));

		var provider = serviceProvider ?? _serviceProvider;
		return provider.GetRequiredService<IServiceScopeFactory>().CreateScope();
	}
}
