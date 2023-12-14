using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace PiranhaCMS.Common.Extensions;

public static class PageExtensions
{
	public static PageBase? GetParentPage(this PageBase page, bool draft = false)
	{
		using var serviceScope = ServiceActivator.GetScope();
		var loader = serviceScope.ServiceProvider.GetRequiredService<IModelLoader>();
		var httpContextAccessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

		return page.ParentId.HasValue ?
			loader.GetPageAsync<PageBase>(page.ParentId.Value, httpContextAccessor.HttpContext.User, draft).GetAwaiter().GetResult() :
			null;
	}

	public static T Get<T>(this PageBase page, bool draft = false) where T : PageBase
	{
		using var serviceScope = ServiceActivator.GetScope();
		var loader = serviceScope.ServiceProvider.GetRequiredService<IModelLoader>();
		var httpContextAccessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

		return loader.GetPageAsync<T>(page.Id, httpContextAccessor.HttpContext.User, draft).GetAwaiter().GetResult();
	}

	public static IEnumerable<SitemapItem> GetChildrenPages(this PageBase page)
	{
		using var serviceScope = ServiceActivator.GetScope();
		var webApp = serviceScope.ServiceProvider.GetRequiredService<IApplicationService>();
		var httpContextAccessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

		webApp.InitAsync(httpContextAccessor.HttpContext).GetAwaiter().GetResult();

		return webApp.Site.Sitemap.GetPartial(page.Id);
	}

	public static IEnumerable<SitemapItem> GetSameLevelPages(this PageBase page)
	{
		if (!page.ParentId.HasValue) return Enumerable.Empty<SitemapItem>();

		using var serviceScope = ServiceActivator.GetScope();
		var webApp = serviceScope.ServiceProvider.GetRequiredService<IApplicationService>();
		var httpContextAccessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

		webApp.InitAsync(httpContextAccessor.HttpContext).GetAwaiter().GetResult();

		return webApp.Site.Sitemap.GetPartial(page.ParentId).Where(x => x.Id != page.Id);
	}

	public static IEnumerable<PageBase> GetDescendants(this PageBase page)
	{
		if (page == null)
			yield break;

		using var serviceScope = ServiceActivator.GetScope();
		var loader = serviceScope.ServiceProvider.GetRequiredService<IModelLoader>();
		var httpContextAccessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
		var currentPage = page;

		while (currentPage.ParentId != null)
		{
			currentPage = loader.GetPageAsync<PageBase>(currentPage.ParentId.Value, httpContextAccessor.HttpContext.User, false).GetAwaiter().GetResult();
			yield return currentPage;
		}
	}

	public static Site GetCurrentSite(this PageBase page)
	{
		using var serviceScope = ServiceActivator.GetScope();
		var api = serviceScope.ServiceProvider.GetRequiredService<IApi>();

		return api.Sites.GetByIdAsync(page.SiteId).GetAwaiter().GetResult();
	}
}
