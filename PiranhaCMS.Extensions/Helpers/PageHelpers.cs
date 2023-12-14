using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace PiranhaCMS.Common.Helpers;

public static class PageHelpers
{
	public static PageBase? GetCurrentPage()
	{
		try
		{
			using var serviceScope = ServiceActivator.GetScope();
			var webApp = serviceScope.ServiceProvider.GetRequiredService<IApplicationService>();
			var httpContext = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

			webApp.InitAsync(httpContext.HttpContext).GetAwaiter().GetResult();

			if (!string.IsNullOrEmpty(httpContext.HttpContext?.Request.Query["id"]))
			{
				return webApp.Api.Pages.GetByIdAsync<PageBase>(Guid.Parse(httpContext.HttpContext.Request.Query["id"])).GetAwaiter().GetResult();
			}

			return null;
		}
		catch
		{
			return null;
		}
	}

	public static string GetSearchQuery()
	{
		using var serviceScope = ServiceActivator.GetScope();
		var httpContext = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

		if (!string.IsNullOrEmpty(httpContext?.HttpContext?.Request?.Query["q"]))
		{
			return httpContext.HttpContext.Request.Query["q"].ToString();
		}

		return string.Empty;
	}

	public static T? GetPageById<T>(Guid id) where T : PageBase
	{
		using var serviceScope = ServiceActivator.GetScope();
		var httpContextAccessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
		var modelLoader = serviceScope.ServiceProvider.GetRequiredService<IModelLoader>();
		var page = modelLoader.GetPageAsync<T>(id, httpContextAccessor.HttpContext?.User, false).GetAwaiter().GetResult();

		return page ?? default;
	}
}
