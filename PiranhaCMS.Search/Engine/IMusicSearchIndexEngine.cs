using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Requests;

namespace PiranhaCMS.Search.Engine;

public interface ISearchIndexEngine<T> where T : IDocument
{
    bool IndexNotExistsOrEmpty();
    SearchResultDto<T> Search(SearchRequest request);
    IDictionary<string, int> CountDocuments(CounterRequest? request);
    IDictionary<string, string> GetLatestAddedItems(CounterRequest request);
}
