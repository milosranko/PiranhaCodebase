using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.ContentTypes.Sites;
using PiranhaCMS.PublicWeb.Models.ViewModels;

namespace PiranhaCMS.PublicWeb.Business.Filters;

public class PageContextActionFilter : IResultFilter
{
    private readonly IApplicationService _applicationService;
    private const string ManagerAreaName = "Manager";

    public PageContextActionFilter(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.RouteData.Values["area"]?.ToString() == ManagerAreaName) return;

        var viewModel = (context.Result as ViewResult)?.Model;

        if (!(viewModel is IPageViewModel<IPage>)) return;

        var siteGlobal = _applicationService.Api.Sites.GetDefaultAsync().GetAwaiter().GetResult();
        var site = _applicationService.Site
            .GetContentAsync<PublicSite>()
            .GetAwaiter()
            .GetResult();
        var startPage = _applicationService.Api.Pages
            .GetStartpageAsync()
            .GetAwaiter()
            .GetResult();
        var model = viewModel as IPageViewModel<IPage>;

        model.GlobalSettings = new GlobalSettingsViewModel
        {
            LanguageCode = _applicationService.Site.Culture,
            StartPageId = startPage.Id,
            EmailAddress = site.GlobalSettings?.EmailAddress?.Value,
            PhoneNumber = site.GlobalSettings?.PhoneNumber?.Value,
            SiteTitle = site.Title,
            PageTitle = model.CurrentPage is StartPage
                ? site.Title
                : $"{(model.CurrentPage as PageBase)?.Title} | {site.Title}"
        };
        model.Footer = new FooterViewModel
        {
            Column1Header = site.SiteFooter?.Column1Header?.Value,
            Column1Content = site.SiteFooter?.Column1Content?.Value,
            Column2Header = site.SiteFooter?.Column2Header?.Value,
            Column2Content = site.SiteFooter?.Column2Content?.Value,
            Column3Header = site.SiteFooter?.Column3Header?.Value,
            Column3Content = site.SiteFooter?.Column3Content?.Value,
            Column4Header = site.SiteFooter?.Column4Header?.Value,
            Column4Content = site.SiteFooter?.Column4Content?.Value
        };
        model.Header = new HeaderViewModel
        {
            SiteLogoImageUrl = site.GlobalSettings?.LogoImage == null
                ? siteGlobal.Logo?.Media?.PublicUrl
                : site.GlobalSettings.LogoImage.Media?.PublicUrl,
            TopLinks = site.TopLinks,
            EmailAddress = site.GlobalSettings?.EmailAddress?.Value,
            PhoneNumber = site.GlobalSettings?.PhoneNumber?.Value
        };
    }

    public void OnResultExecuted(ResultExecutedContext context)
    { }
}
