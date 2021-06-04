using Microsoft.AspNetCore.Http;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using System.Collections.Generic;
using System.Linq;

namespace PiranhaCMS.Common.Extensions
{
    public static class PageExtensions
    {
        public static PageBase GetParentPage(this PageBase page)
        {
            using var serviceScope = ServiceActivator.GetScope();
            var loader = (IModelLoader)serviceScope.ServiceProvider.GetService(typeof(IModelLoader));
            var httpContextAccessor = (IHttpContextAccessor)serviceScope.ServiceProvider.GetService(typeof(IHttpContextAccessor));

            return page.ParentId.HasValue ?
                loader.GetPageAsync<PageBase>(page.ParentId.Value, httpContextAccessor.HttpContext.User, false).GetAwaiter().GetResult() :
                null;
        }

        public static T Get<T>(this PageBase page) where T : PageBase
        {
            using var serviceScope = ServiceActivator.GetScope();
            var loader = (IModelLoader)serviceScope.ServiceProvider.GetService(typeof(IModelLoader));
            var httpContextAccessor = (IHttpContextAccessor)serviceScope.ServiceProvider.GetService(typeof(IHttpContextAccessor));

            return loader.GetPageAsync<T>(page.Id, httpContextAccessor.HttpContext.User, false).GetAwaiter().GetResult();
        }

        public static IEnumerable<SitemapItem> GetChildrenPages(this PageBase page)
        {
            using var serviceScope = ServiceActivator.GetScope();
            var webApp = (IApplicationService)serviceScope.ServiceProvider.GetService(typeof(IApplicationService));
            var httpContextAccessor = (IHttpContextAccessor)serviceScope.ServiceProvider.GetService(typeof(IHttpContextAccessor));

            webApp.InitAsync(httpContextAccessor.HttpContext).GetAwaiter().GetResult();

            return webApp.Site.Sitemap.GetPartial(page.Id);
        }

        public static IEnumerable<SitemapItem> GetSameLevelPages(this PageBase page)
        {
            if (!page.ParentId.HasValue) return Enumerable.Empty<SitemapItem>();

            using var serviceScope = ServiceActivator.GetScope();
            var webApp = (IApplicationService)serviceScope.ServiceProvider.GetService(typeof(IApplicationService));
            var httpContextAccessor = (IHttpContextAccessor)serviceScope.ServiceProvider.GetService(typeof(IHttpContextAccessor));

            webApp.InitAsync(httpContextAccessor.HttpContext).GetAwaiter().GetResult();

            return webApp.Site.Sitemap.GetPartial(page.ParentId).Where(x => x.Id != page.Id);
        }

        public static Site GetCurrentSite(this PageBase page)
        {
            using var serviceScope = ServiceActivator.GetScope();
            var api = (IApi)serviceScope.ServiceProvider.GetService(typeof(IApi));

            return api.Sites.GetByIdAsync(page.SiteId).GetAwaiter().GetResult();
        }
    }
}
