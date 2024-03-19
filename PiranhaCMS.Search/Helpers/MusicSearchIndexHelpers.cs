using Microsoft.Extensions.Logging;
using Piranha.Cache;
using Piranha.Models;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Models.Constants;
using System.IO.Compression;

namespace PiranhaCMS.Search.Helpers;

internal class MusicSearchIndexHelpers : IMusicSearchIndexHelpers
{
    private readonly ICache _cache;
    private readonly IMusicSearchIndexEngine _engine;
    private readonly ILogger<MusicSearchIndexHelpers> _logger;

    public MusicSearchIndexHelpers(
        ICache cache,
        IMusicSearchIndexEngine engine,
        ILogger<MusicSearchIndexHelpers> logger)
    {
        _cache = cache;
        _engine = engine;
        _logger = logger;
    }

    public void ExtractMLA(Media media)
    {
        if (media == null || !media.Filename.EndsWith(".mla"))
            return;

        var path = Path.Combine(Environment.CurrentDirectory, _engine.GetIndexName());

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var uploadPath = Path.Combine("wwwroot", "uploads", media.Id.ToString() + "-" + media.Filename);

        _logger.LogDebug("Unzipping file...");

        using var archive = ZipFile.OpenRead(uploadPath);

        foreach (var entry in archive.Entries)
            entry.ExtractToFile(Path.GetFullPath(Path.Combine(path, entry.FullName)), true);

        //Invalidate cache
        _logger.LogDebug("Invalidating cache...");
        _cache.Remove(CacheKeys.MusicIndexCount);
        _cache.Set(CacheKeys.MusicIndexCount, _engine.GetIndexStatistics());
    }
}

internal interface IMusicSearchIndexHelpers
{
    void ExtractMLA(Media media);
}