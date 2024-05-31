using Lucene.Net.Index;
using PiranhaCMS.Search.Models.Internal;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

internal interface IDocumentReader : IDisposable
{
    bool IndexNotExistsOrEmpty();
    SearchResultInternal Search(SearchRequestInternal request);
    void Init(Lucene.Net.Store.Directory directory, Lucene.Net.Store.Directory taxoDirectory);
    DirectoryReader? Reader { get; }
    IDictionary<string, int> TermsCounter(string field, bool isNumeric = false);
    IDictionary<string, string> LatestAdded(string field, string additionalField, string sortBy, ListSortDirection sortDirection, int top);
}
