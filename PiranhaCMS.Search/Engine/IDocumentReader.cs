using Lucene.Net.Index;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

internal interface IDocumentReader : IDisposable
{
    bool IndexNotExistsOrEmpty();
    SearchResult Search(SearchRequest request);
    void Init();
    DirectoryReader? Reader { get; }
    IDictionary<string, int> TermsCounter(string field, bool isNumeric = false);
    IDictionary<string, string> LatestAdded(string field, string additionalField, string sortBy, ListSortDirection sortDirection, int top);
}
