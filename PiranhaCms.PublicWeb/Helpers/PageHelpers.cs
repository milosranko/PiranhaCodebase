using Microsoft.AspNetCore.Http;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using PiranhaCMS.Common;
using PiranhaCMS.PublicWeb.Models.Sites;
using System;

namespace PiranhaCMS.PublicWeb.Helpers
{
    public static class PageHelpers
    {
        public static GlobalSettings GetSiteSettings()
        {
            using var serviceScope = ServiceActivator.GetScope();
            var webApp = (IApplicationService)serviceScope.ServiceProvider.GetService(typeof(IApplicationService));
            var httpContext = (IHttpContextAccessor)serviceScope.ServiceProvider.GetService(typeof(IHttpContextAccessor));

            webApp.InitAsync(httpContext.HttpContext).GetAwaiter().GetResult();

            var site = webApp.Site
                .GetContentAsync<PublicSite>()
                .GetAwaiter()
                .GetResult();

            return site.GlobalSettings;
        }

        public static PageBase GetCurrentPage()
        {
            try
            {
                using var serviceScope = ServiceActivator.GetScope();
                var webApp = (IApplicationService)serviceScope.ServiceProvider.GetService(typeof(IApplicationService));
                var httpContext = (IHttpContextAccessor)serviceScope.ServiceProvider.GetService(typeof(IHttpContextAccessor));

                webApp.InitAsync(httpContext.HttpContext).GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(httpContext.HttpContext.Request.Query["id"]))
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
    }
}
