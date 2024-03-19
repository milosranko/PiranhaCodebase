using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Services;
using Piranha.Cache;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.PublicWeb.Models.ViewModels;
using PiranhaCMS.Search.Engine;

namespace PiranhaCMS.PublicWeb.Controllers;

[ResponseCache(Duration = 36000, VaryByQueryKeys = ["id"])]
public class CmsController : Controller
{
    private readonly IModelLoader _loader;
    private readonly ISearchIndexEngine _searchIndexEngine;
    private readonly ICache _cache;

    public CmsController(IModelLoader loader, ISearchIndexEngine searchIndexEngine, ICache cache)
    {
        _loader = loader;
        _searchIndexEngine = searchIndexEngine;
        _cache = cache;
    }

    [Route(nameof(StartPage))]
    public async Task<IActionResult> StartPage(Guid id, bool draft = false)
    {
        var currentPage = await _loader.GetPageAsync<StartPage>(id, HttpContext.User, draft);
        var viewModel = new StartPageViewModel(currentPage);

        return View(viewModel);
    }

    [Route(nameof(ArticlePage))]
    public async Task<IActionResult> ArticlePage(Guid id, bool draft = false)
    {
        var currentPage = await _loader.GetPageAsync<ArticlePage>(id, HttpContext.User, draft);
        var viewModel = new ArticlePageViewModel(currentPage);

        return View(viewModel);
    }

    [Route(nameof(ArticleListPage))]
    public async Task<IActionResult> ArticleListPage(Guid id, bool draft = false)
    {
        var currentPage = await _loader.GetPageAsync<ArticleListPage>(id, HttpContext.User, draft);
        var viewModel = new ArticleListPageViewModel(currentPage);

        return View(viewModel);
    }

    [ResponseCache(Duration = 36000, VaryByQueryKeys = ["id", "q"])]
    [Route(nameof(SearchPage))]
    public async Task<IActionResult> SearchPage(Guid id, bool draft = false)
    {
        var currentPage = await _loader.GetPageAsync<SearchPage>(id, HttpContext.User, draft);
        var viewModel = new SearchPageViewModel(currentPage, HttpContext.Request, _searchIndexEngine);

        return View(viewModel);
    }

    [ResponseCache(NoStore = true)]
    [Route(nameof(MusicSearchPage))]
    public async Task<IActionResult> MusicSearchPage(Guid id, bool draft = false)
    {
        var currentPage = await _loader.GetPageAsync<MusicSearchPage>(id, HttpContext.User, draft);
        var viewModel = new MusicSearchPageViewModel(currentPage, HttpContext, _cache);

        return View(viewModel);
    }

    [Route(nameof(NotFoundPage))]
    public async Task<IActionResult> NotFoundPage(Guid id, bool draft = false)
    {
        var currentPage = await _loader.GetPageAsync<NotFoundPage>(id, HttpContext.User, draft);
        var viewModel = new NotFoundPageViewModel(currentPage);

        return View(viewModel);
    }
}
