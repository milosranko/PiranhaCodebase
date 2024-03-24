using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Requests;

namespace PiranhaCMS.Search.Engine;

public interface ISearchIndexEngine<T> where T : IDocument
{
    IEnumerable<T> GetByIds(string[] ids);
    bool IndexNotExistsOrEmpty();
    IEnumerable<string> SkipExistingDocuments(string[] ids);
    SearchResultDto<T> Search(SearchRequest request);
    IDictionary<string, int> CountDocuments(CounterRequest? request);
    IDictionary<string, string> GetLatestAddedItems(CounterRequest request);
    IEnumerable<string> GetAllIndexedIds();
}
