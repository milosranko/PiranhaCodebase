using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Helpers;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Config;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Services;

namespace PiranhaCMS.Search.Startup;

public static class StartupExtensions
{
    public static IServiceCollection AddPiranhaSearch(
        this IServiceCollection services,
        Action<PiranhaSearchServiceBuilder> options)
    {
        var serviceBuilder = new PiranhaSearchServiceBuilder(services);
        options?.Invoke(serviceBuilder);

        App.Modules.Register<Module>();

        services.AddSingleton<ISearchIndexEngine>(x => new SearchIndexEngine(serviceBuilder));
        services.AddSingleton<ISearch, SearchIndexService>();

        return serviceBuilder.Services;
    }

    public static IServiceCollection AddMusicSearch(
        this IServiceCollection services,
        Action<PiranhaSearchServiceBuilder> options)
    {
        var serviceBuilder = new PiranhaSearchServiceBuilder(services);
        options?.Invoke(serviceBuilder);

        services.AddSingleton<ISearchIndexEngine<MusicLibraryDocument>>(x => new MusicSearchIndexEngine<MusicLibraryDocument>(serviceBuilder));

        if (serviceBuilder.StorageType == IndexDirectory.Azure)
            services.AddSingleton<IMusicSearchIndexHelpers, AzureMusicSearchIndexHelpers>();
        else
            services.AddSingleton<IMusicSearchIndexHelpers, MusicSearchIndexHelpers>();

        return services;
    }

    public static IApplicationBuilder UseMusicSearch(this IApplicationBuilder app)
    {
        if (!App.MediaTypes.Documents.ContainsExtension(".mla"))
            App.MediaTypes.Documents.Add(".mla", "application/mla", false);

        var musicSearchIndexHelpers = app.ApplicationServices.GetRequiredService<IMusicSearchIndexHelpers>();
        App.Hooks.Media.RegisterOnAfterSave(musicSearchIndexHelpers.ExtractMLA);

        return app;
    }

    public static IApplicationBuilder UsePiranhaSearch(
        this IApplicationBuilder app,
        IApi api,
        Action<PiranhaSearchApplicationBuilder> options)
    {
        var applicationBuilder = new PiranhaSearchApplicationBuilder();
        options?.Invoke(applicationBuilder);

        var searchIndexEngine = app.ApplicationServices.GetService<ISearchIndexEngine>() ?? throw new Exception("Search engine not initialized!");
        _ = new SearchOptions(applicationBuilder.Include);

        var pagesIndexed = IndexSite(searchIndexEngine, api, applicationBuilder.ForceReindexing)
            .GetAwaiter()
            .GetResult();

        return app;
    }

    private static async Task<int> IndexSite(
        ISearchIndexEngine searchIndexEngine,
        IApi api,
        bool forceReindexing)
    {
        if (!forceReindexing && searchIndexEngine.IndexExists()) return 0;
        if (forceReindexing) searchIndexEngine.DeleteAll();

        var defaultSite = await api.Sites.GetDefaultAsync();
        var pages = await api.Pages.GetAllAsync(defaultSite?.Id);

        if (pages == null) return 0;

        var counter = 0;
        foreach (var page in pages)
        {
            if (!SearchOptions.Include.Any(x => x.Name.Equals(page.TypeId))) continue;
            if (!page.IsPublished || page.Permissions.Any()) continue;

            var body = PageContentHelpers.ExtractPageContent(page);
            var content = new Content
            {
                ContentId = page.Id.ToString(),
                Title = page.Title,
                RouteName = page.Slug,
                ContentType = page.TypeId,
                Url = page.Permalink,
                Text = body,
                Category = page.TypeId
            };

            searchIndexEngine.AddToIndexWithoutCommit(content);
            counter++;
        }

        searchIndexEngine.Commit();
        return counter;
    }
}
