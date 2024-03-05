using PiranhaCMS.Search.Models;

namespace PiranhaCMS.Search.Engine;

public interface IMusicSearchIndexEngine
{
    MusicSearchResult Search(MusicSearchRequest request);
    bool IndexNotExistsOrEmpty();
    MusicIndexCounts GetIndexStatistics();
    string GetIndexName();
}
