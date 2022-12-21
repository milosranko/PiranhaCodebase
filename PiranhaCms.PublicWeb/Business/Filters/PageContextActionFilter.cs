using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.PublicWeb.Models.Sites;
using PiranhaCMS.PublicWeb.Models.ViewModels;

namespace PiranhaCMS.PublicWeb.Business.Filters
{
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

            var model = viewModel as IPageViewModel<IPage>;
            var startPage = _applicationService.Api.Pages
                .GetStartpageAsync()
                .GetAwaiter()
                .GetResult();
            var siteGlobal = _applicationService.Api.Sites.GetDefaultAsync().GetAwaiter().GetResult();
            var site = _applicationService.Site
                .GetContentAsync<PublicSite>()
                .GetAwaiter()
                .GetResult();

            model.Footer = new FooterViewModel
            {
                FooterColumn1 = site.SiteFooter?.FooterColumn1?.Value,
                FooterColumn2 = site.SiteFooter?.FooterColumn2?.Value,
                SiteTitle = site.Title
            };

            model.Header = new HeaderViewModel
            {
                SiteName = site.SiteHeader.SiteName,
                SiteLogoImageUrl = siteGlobal.Logo?.Media?.PublicUrl //site.SiteHeader.LogoImage?.Media?.PublicUrl
            };

            model.PageTitle = model.CurrentPage is StartPage ?
                site.SiteHeader.SiteName.Value :
                $"{(model.CurrentPage as PageBase)?.Title} | {site.SiteHeader.SiteName.Value}";

            model.StartPageId = startPage.Id;
            model.LanguageCode = _applicationService.Site.Culture;

            //_applicationService.Api.Sites
            //.GetByHostnameAsync(context.HttpContext.Request.Host.Value)
            //.GetAwaiter()
            //.GetResult()
            //.Title;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        { }
    }
}
