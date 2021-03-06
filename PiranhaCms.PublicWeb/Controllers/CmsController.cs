using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Services;
using PiranhaCMS.PublicWeb.Models.Pages;
using PiranhaCMS.PublicWeb.Models.ViewModels;
using PiranhaCMS.Search.Engine;
using System;
using System.Threading.Tasks;

namespace PiranhaCMS.PublicWeb.Controllers
{
    public class CmsController : Controller
    {
        private readonly IModelLoader _loader;
        private readonly ISearchIndexEngine _searchIndexEngine;

        public CmsController(IModelLoader loader, ISearchIndexEngine searchIndexEngine)
        {
            _loader = loader;
            _searchIndexEngine = searchIndexEngine;
        }

        [Route("startpage")]
        public async Task<IActionResult> StartPage(Guid id, bool draft = false)
        {
            var currentPage = await _loader.GetPageAsync<StartPage>(id, HttpContext.User, draft);
            var viewModel = new StartPageViewModel(currentPage);

            return View(viewModel);
        }

        [Route("articlepage")]
        public async Task<IActionResult> ArticlePage(Guid id, bool draft = false)
        {
            var currentPage = await _loader.GetPageAsync<ArticlePage>(id, HttpContext.User, draft);
            var viewModel = new ArticlePageViewModel(currentPage);

            return View(viewModel);
        }

        [Route("searchpage")]
        public async Task<IActionResult> SearchPage(Guid id, bool draft = false)
        {
            var currentPage = await _loader.GetPageAsync<SearchPage>(id, HttpContext.User, draft);
            var viewModel = new SearchPageViewModel(currentPage, HttpContext.Request, _searchIndexEngine);

            return View(viewModel);
        }

        [Route("notfoundpage")]
        public async Task<IActionResult> NotFoundPage(Guid id, bool draft = false)
        {
            var currentPage = await _loader.GetPageAsync<NotFoundPage>(id, HttpContext.User, draft);
            var viewModel = new NotFoundPageViewModel(currentPage);

            return View(viewModel);
        }
    }
}
