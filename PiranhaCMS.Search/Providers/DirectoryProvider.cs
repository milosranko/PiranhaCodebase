using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Startup;
using Directory = Lucene.Net.Store.Directory;

namespace PiranhaCMS.Search.Providers;

internal static class DirectoryProvider
{
    private const string FacetsIndexFolderName = "Facets";

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
                return new AzureDirectory(serviceBuilder.AzureStorageCredentials, "");
            default:
                return new RAMDirectory();
        }
    }

    public static Directory CreateFacetIndex(PiranhaSearchServiceBuilder serviceBuilder)
    {
        var path = Path.Combine(serviceBuilder.IndexDirectory, FacetsIndexFolderName);

        switch (serviceBuilder.StorageType)
        {
            case IndexDirectory.FileSystem:
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return FSDirectory.Open(path);
            case IndexDirectory.Memory:
                return new RAMDirectory();
            case IndexDirectory.Azure:
                return new AzureDirectory(serviceBuilder.AzureStorageCredentials, "");
            default:
                return new RAMDirectory();
        }
    }
}
