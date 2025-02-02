using Piranha.Models;

namespace PiranhaCMS.ImageCache.Services;

public interface IImageCacheService
{
    Task<Guid> GetOrCreateDefaultImageCacheFolder();
    void ConvertToWebP(Media media);
    void ConvertToWebP(Media media, out Guid id);
    string ConvertToWebP(Media media, int width, int height);
    Media GetById(Guid id);
}
