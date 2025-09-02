using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Piranha.Cache;
using Piranha.Models;
using PiranhaCMS.Search.Models.Constants;
using System.IO.Compression;

namespace PiranhaCMS.Search.Helpers;

internal class AzureMusicSearchIndexHelpers : IMusicSearchIndexHelpers
{
    private readonly ICache _cache;
    private readonly ILogger<MusicSearchIndexHelpers> _logger;
    private readonly IConfiguration _configuration;

    public AzureMusicSearchIndexHelpers(
        ICache cache,
        ILogger<MusicSearchIndexHelpers> logger,
        IConfiguration configuration)
    {
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
    }

    public void ExtractMLA(Media media)
    {
        if (media == null || !media.Filename.EndsWith(".mla"))
            return;

        var blobServiceClient = new BlobServiceClient(_configuration["Piranha:StorageConnectionString"]);
        var uploadsContainer = blobServiceClient.GetBlobContainerClient("uploads");
        var luceneContainer = blobServiceClient.GetBlobContainerClient("music-lucene");
        var taxoContainer = blobServiceClient.GetBlobContainerClient("music-lucene-taxo");
        var zipSource = uploadsContainer.GetBlobClient(media.Id.ToString() + "-" + media.Filename);

        using var zipMemStream = new MemoryStream();
        zipSource.DownloadTo(zipMemStream);
        zipMemStream.Position = 0;

        // Ensure containers exist
        luceneContainer.CreateIfNotExists();
        taxoContainer.CreateIfNotExists();

        _logger.LogDebug("Unzipping file...");

        using (var archive = new ZipArchive(zipMemStream, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries.Where(x => x.FullName.Contains("music-library/", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (!string.IsNullOrEmpty(entry.Name)) // Only process actual files, not directories
                {
                    var destinationBlob = luceneContainer.GetBlobClient(entry.Name);

                    using (var entryStream = entry.Open())
                    {
                        destinationBlob.Upload(entryStream, overwrite: true);
                    }
                }
            }

            foreach (var entry in archive.Entries.Where(x => x.FullName.Contains("music-library-taxo/", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (!string.IsNullOrEmpty(entry.Name)) // Only process actual files, not directories
                {
                    var destinationBlob = taxoContainer.GetBlobClient(entry.Name);

                    using (var entryStream = entry.Open())
                    {
                        destinationBlob.Upload(entryStream, overwrite: true);
                    }
                }
            }
        }

        _logger.LogDebug("Unzip file finished.");

        //Invalidate cache
        _cache.RemoveAsync(CacheKeys.MusicIndexCount).GetAwaiter().GetResult();
        //_cache.Set(CacheKeys.MusicIndexCount, _engine.CountDocuments());
    }
}
