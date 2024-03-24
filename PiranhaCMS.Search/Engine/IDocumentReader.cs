using Lucene.Net.Documents;
using Lucene.Net.Index;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

internal interface IDocumentReader : IDisposable
{
    IEnumerable<Document> GetByIds(string[] ids);
    bool DocumentExists(string id);
    bool IndexNotExistsOrEmpty();
    SearchResult Search(SearchRequest request);
    void Init();
    void Init(DirectoryReader reader);
    DirectoryReader? Reader { get; }
    IDictionary<string, int> TermsCounter(string field, bool isNumeric = false);
    IDictionary<string, string> LatestAdded(string field, string additionalField, string sortBy, ListSortDirection sortDirection, int top);
}
