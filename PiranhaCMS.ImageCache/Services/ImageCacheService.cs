using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.Models;
using PiranhaCMS.Common;
using SixLabors.ImageSharp;

namespace PiranhaCMS.ImageCache.Services;

internal class ImageCacheService : IImageCacheService
{
    private const string WEBP_EXTENSION = ".webp";
    private readonly ILogger<ImageCacheService> _logger;

    public ImageCacheService(ILogger<ImageCacheService> logger)
    {
        _logger = logger;
    }

    public void ConvertToWebP(Media media)
    {
        //TODO Check ImageCacheOptionsBuilder for file allowed file extensions, implement IOptions pattern
        if (media == null || media.Type != MediaType.Image || !(media.Filename.EndsWith(".jpg") || media.Filename.EndsWith(".jpeg") || media.Filename.EndsWith(".png")))
            return;

        try
        {
            using var serviceScope = ServiceActivator.GetScope();
            var _storage = serviceScope.ServiceProvider.GetRequiredService<IStorage>();
            var _api = serviceScope.ServiceProvider.GetRequiredService<IApi>();

            using var stream = new MemoryStream();
            using var session = _storage.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (!session.GetAsync(media, media.Filename, stream).GetAwaiter().GetResult())
                return;

            stream.Position = 0;

            using var output = new MemoryStream();
            using (var image = Image.Load(stream))
                image.SaveAsWebp(output);

            if (output.Length >= stream.Length)
                return;

            output.Position = 0;

            var newFileName = media.Filename.Replace(media.Filename[media.Filename.LastIndexOf(".")..], WEBP_EXTENSION);
            var type = App.MediaTypes.GetItem(newFileName);
            var mediaContent = new StreamMediaContent
            {
                Filename = newFileName,
                Data = output,
                FolderId = media.FolderId
            };

            _api.Media.SaveAsync(mediaContent).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting media file to WebP format: {0}", media.Filename);
            throw;
        }
    }
}
