using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Startup;
using Directory = Lucene.Net.Store.Directory;

namespace PiranhaCMS.Search.Providers;

internal static class DirectoryProvider
{
    private const string FACETS_INDEX_FOLDER_NAME = "taxo";

    public static Directory CreateDocumentIndex(PiranhaSearchServiceBuilder serviceBuilder)
    {
        switch (serviceBuilder.StorageType)
        {
            case IndexDirectory.FileSystem:
                if (!System.IO.Directory.Exists(serviceBuilder.IndexDirectory))
                    System.IO.Directory.CreateDirectory(serviceBuilder.IndexDirectory);

                return FSDirectory.Open(serviceBuilder.IndexDirectory);
            case IndexDirectory.Memory:
                return new RAMDirectory();
            case IndexDirectory.Azure:
                return new AzureDirectory(serviceBuilder.AzureStorageCredentials, serviceBuilder.IndexDirectory);
            default:
                return new RAMDirectory();
        }
    }

    public static Directory CreateFacetIndex(PiranhaSearchServiceBuilder serviceBuilder)
    {
        switch (serviceBuilder.StorageType)
        {
            case IndexDirectory.FileSystem:
                var path = Path.Combine(serviceBuilder.IndexDirectory, FACETS_INDEX_FOLDER_NAME);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return FSDirectory.Open(path);
            case IndexDirectory.Memory:
                return new RAMDirectory();
            case IndexDirectory.Azure:
                return new AzureDirectory(serviceBuilder.AzureStorageCredentials, $"{serviceBuilder.IndexDirectory}-{FACETS_INDEX_FOLDER_NAME}");
            default:
                return new RAMDirectory();
        }
    }
}
