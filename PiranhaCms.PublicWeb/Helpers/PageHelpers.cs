using Microsoft.AspNetCore.Http;
using Piranha.AspNetCore.Services;
using PiranhaCMS.Common;
using PiranhaCMS.PublicWeb.Models.Sites;

namespace PiranhaCMS.PublicWeb.Helpers;

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
}
