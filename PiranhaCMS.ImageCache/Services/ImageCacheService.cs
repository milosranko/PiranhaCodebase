using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.Models;
using PiranhaCMS.ImageCache.Models;
using PiranhaCMS.ImageCache.Startup;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace PiranhaCMS.ImageCache.Services;

internal class ImageCacheService : IImageCacheService
{
    private readonly ImageCacheOptionsBuilder _options;
    private readonly ILogger<ImageCacheService> _logger;
    private readonly IApi _api;
    private readonly IStorage _storage;

    public ImageCacheService(ILogger<ImageCacheService> logger, ImageCacheOptionsBuilder options, IApi api, IStorage storage)
    {
        _logger = logger;
        _options = options;
        _api = api;
        _storage = storage;
    }

    public void ConvertToWebP(Media media)
    {
        if (media == null ||
            media.Type != MediaType.Image ||
            !_options.FileExtensionsForWebP.Any(x => x.Equals(Path.GetExtension(media.Filename))) ||
            media.FolderId.Equals(_options.DefaultImageCacheFolderId))
            return;

        try
        {
            using var originalImageStream = new MemoryStream();
            using var session = _storage.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (!session.GetAsync(media, media.Filename, originalImageStream).GetAwaiter().GetResult())
                return;

            originalImageStream.Position = 0;

            using var output = new MemoryStream();
            using (var image = Image.Load(originalImageStream))
                image.SaveAsWebp(output, new WebpEncoder
                {
                    Quality = (int)_options.Quality
                });

            if (output.Length > originalImageStream.Length)
                return;

            output.Position = 0;

            var newFileName = media.Filename.Replace(media.Filename[media.Filename.LastIndexOf(".")..], StringConstants.WEBP_EXTENSION);
            var mediaContent = new StreamMediaContent
            {
                Filename = newFileName,
                Data = output,
                FolderId = _options.DefaultImageCacheFolderId
            };

            _api.Media.SaveAsync(mediaContent).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting media file to WebP format: {0}", media.Filename);
            throw;
        }
    }

    public void ConvertToWebP(Media media, out Guid id)
    {
        id = Guid.Empty;

        if (media == null ||
            media.Type != MediaType.Image ||
            !_options.FileExtensionsForWebP.Any(x => x.Equals(Path.GetExtension(media.Filename))) ||
            media.FolderId.Equals(_options.DefaultImageCacheFolderId))
            return;

        var existingImage = _api.Media.GetAllByFolderIdAsync(_options.DefaultImageCacheFolderId).GetAwaiter().GetResult()
            .FirstOrDefault(x => x.Filename.StartsWith(Path.GetFileNameWithoutExtension(media.Filename)));

        if (existingImage is not null)
        {
            id = existingImage.Id;
            return;
        }

        try
        {
            using var originalImageStream = new MemoryStream();
            using var session = _storage.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (!session.GetAsync(media, media.Filename, originalImageStream).GetAwaiter().GetResult())
                return;

            originalImageStream.Position = 0;

            using var output = new MemoryStream();
            using (var image = Image.Load(originalImageStream))
                image.SaveAsWebp(output, new WebpEncoder
                {
                    Quality = (int)_options.Quality
                });

            if (output.Length > originalImageStream.Length)
                return;

            output.Position = 0;

            var newFileName = media.Filename.Replace(media.Filename[media.Filename.LastIndexOf(".")..], StringConstants.WEBP_EXTENSION);
            var mediaContent = new StreamMediaContent
            {
                Filename = newFileName,
                Data = output,
                FolderId = _options.DefaultImageCacheFolderId
            };

            _api.Media.SaveAsync(mediaContent).GetAwaiter().GetResult();

            id = mediaContent.Id ?? Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting media file to WebP format: {0}", media.Filename);
            throw;
        }
    }

    public string ConvertToWebP(Media media, int width, int height)
    {
        if (media == null ||
            media.Type != MediaType.Image ||
            !_options.FileExtensionsForWebP.Any(x => x.Equals(Path.GetExtension(media.Filename))) ||
            media.FolderId.Equals(_options.DefaultImageCacheFolderId) ||
            (width.Equals(0) && height.Equals(0)))
            return string.Empty;

        if (media.Versions.Any(x => x.Width.Equals(width) && x.Height.Equals(height) && x.FileExtension.Equals(StringConstants.WEBP_EXTENSION)))
            return _api.Media.EnsureVersion(media.Id, width, height);

        try
        {
            //Create new image version
            using var originalImageStream = new MemoryStream();
            using var session = _storage.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (!session.GetAsync(media, media.Filename, originalImageStream).GetAwaiter().GetResult())
                return string.Empty;

            originalImageStream.Position = 0;

            using var output = new MemoryStream();
            using (var image = Image.Load(originalImageStream))
                image.SaveAsWebp(output, new WebpEncoder
                {
                    Quality = (int)_options.Quality
                });

            //Add image version
            media.Versions.Add(new MediaVersion
            {
                FileExtension = StringConstants.WEBP_EXTENSION,
                Width = width,
                Height = height,
                Id = Guid.NewGuid(),
                Size = output.Length
            });

            _api.Media.SaveAsync(media).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {

        }

        return string.Empty;
    }

    public Media GetById(Guid id)
    {
        return _api.Media.GetByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Guid> GetOrCreateDefaultImageCacheFolder()
    {
        if (_options.DefaultImageCacheFolderId.HasValue)
            return _options.DefaultImageCacheFolderId.Value;

        var mediaStructure = await _api.Media.GetStructureAsync();
        var existingFolder = mediaStructure.SingleOrDefault(x => x.Name.Equals(_options.DefaultImageCacheFolderName));

        if (existingFolder is not null)
            return existingFolder.Id;

        var folderId = Guid.NewGuid();

        await _api.Media.SaveFolderAsync(new MediaFolder
        {
            Name = _options.DefaultImageCacheFolderName,
            Description = "Folder for cached image variants",
            //ParentId = mediaStructure.First().Id,
            Id = folderId
        });

        return folderId;
    }
}
