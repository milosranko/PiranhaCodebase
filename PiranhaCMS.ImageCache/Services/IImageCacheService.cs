using Piranha.Models;

namespace PiranhaCMS.ImageCache.Services;

internal interface IImageCacheService
{
    void ConvertToWebP(Media media);
}
