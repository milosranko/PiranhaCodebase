using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using PiranhaCMS.ImageCache.Services;

namespace PiranhaCMS.ImageCache.Startup;

public static class StartupExtensions
{
    public static IServiceCollection AddImageCache(this IServiceCollection services)
    {
        services.AddSingleton<IImageCacheService, ImageCacheService>();

        return services;
    }

    public static IApplicationBuilder UseImageCache(this IApplicationBuilder app, Action<ImageCacheOptionsBuilder> options)
    {
        var optionsBuilder = new ImageCacheOptionsBuilder();
        options?.Invoke(optionsBuilder);

        if (optionsBuilder.ConvertToWebP)
        {
            var imageCacheService = app.ApplicationServices.GetRequiredService<IImageCacheService>();
            App.Hooks.Media.RegisterOnAfterSave(imageCacheService.ConvertToWebP);
        }

        return app;
    }
}
