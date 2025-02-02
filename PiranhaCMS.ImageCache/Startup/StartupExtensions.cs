using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using PiranhaCMS.ImageCache.Services;

namespace PiranhaCMS.ImageCache.Startup;

public static class StartupExtensions
{
    public static IServiceCollection AddImageCache(this IServiceCollection services)
    {
        services.AddSingleton(x => new ImageCacheOptionsBuilder());
        services.AddScoped<IImageCacheService, ImageCacheService>();

        return services;
    }

    public static IApplicationBuilder UseImageCache(this IApplicationBuilder app, Action<ImageCacheOptionsBuilder> options)
    {
        var optionsBuilder = app.ApplicationServices.GetRequiredService<ImageCacheOptionsBuilder>();
        options?.Invoke(optionsBuilder);

        var imageCacheService = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IImageCacheService>();

        if (optionsBuilder.ConvertToWebP)
            App.Hooks.Media.RegisterOnAfterSave(imageCacheService.ConvertToWebP);

        if (!string.IsNullOrEmpty(optionsBuilder.DefaultImageCacheFolderName))
            optionsBuilder.DefaultImageCacheFolderId = imageCacheService.GetOrCreateDefaultImageCacheFolder().GetAwaiter().GetResult();

        return app;
    }
}
