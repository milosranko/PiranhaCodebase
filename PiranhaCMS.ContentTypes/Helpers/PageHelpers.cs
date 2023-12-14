using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Piranha.AspNetCore.Services;
using PiranhaCMS.Common;
using PiranhaCMS.ContentTypes.Sites;

namespace PiranhaCMS.ContentTypes.Helpers;

public static class PageHelpers
{
	public static GlobalSettings GetSiteSettings()
	{
		using var serviceScope = ServiceActivator.GetScope();
		var webApp = serviceScope.ServiceProvider.GetRequiredService<IApplicationService>();
		var httpContext = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

		webApp.InitAsync(httpContext.HttpContext).GetAwaiter().GetResult();

		var site = webApp.Site
			.GetContentAsync<PublicSite>()
			.GetAwaiter()
			.GetResult();

		return site.GlobalSettings;
	}
}
