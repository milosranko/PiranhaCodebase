using PiranhaCMS.Search.Models;

namespace PiranhaCMS.Search.Engine;

public interface ISearchIndexEngine
{
    void AddToIndex(WebPage webPage);
    void AddToIndex(Content content);
    void AddToIndexWithoutCommit(WebPage webPage);
    void AddToIndexWithoutCommit(Content content);
    SearchResult Search(SearchRequest request);
    void Commit();
    void DeleteAll();
    void DeleteById(string id);
    bool IndexExists();
}
