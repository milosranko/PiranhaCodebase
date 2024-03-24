using Lucene.Net.Index;
using PiranhaCMS.Search.Extensions;
using PiranhaCMS.Search.Helpers;
using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

public class MusicSearchIndexEngine<T> : ISearchIndexEngine<T> where T : MappingDocumentBase<T>, IDocument, new()
{
    private readonly IDocumentReader _documentReader;

    public MusicSearchIndexEngine()
    {
        DocumentModelHelpers<T>.ReflectDocumentFields();

        _documentReader = new DocumentReader(DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
    }

    #region Public methods

    public IEnumerable<string> SkipExistingDocuments(string[] ids)
    {
        if (ids.Length == 0)
            return [];

        var result = new Collection<string>();

        foreach (var id in ids)
            if (_documentReader.DocumentExists(id))
                result.Add(id);

        return result;
    }

    public IEnumerable<T> GetByIds(string[] ids)
    {
        _documentReader.Init();
        return _documentReader.GetByIds(ids).Select(x => new T().MapFromLuceneDocument(x));
    }

    public bool IndexNotExistsOrEmpty()
    {
        _documentReader.Init();
        return _documentReader.IndexNotExistsOrEmpty();
    }

    public SearchResultDto<T> Search(SearchRequest request)
    {
        _documentReader.Init();
        return _documentReader.Search(request).ToDto<T>();
    }

    public IEnumerable<string> GetAllIndexedIds()
    {
        _documentReader.Init();

        var res = new Collection<string>();
        var fields = MultiFields.GetFields(_documentReader.Reader);
        var terms = fields.GetTerms(this.GetFieldName(x => x.Id));
        var termsEnum = terms.GetEnumerator(null);

        while (termsEnum.MoveNext() == true)
            res.Add(termsEnum.Term.Utf8ToString());

        return res;
    }

    public IDictionary<string, int> CountDocuments(CounterRequest? request)
    {
        _documentReader.Init();

        if (request is null && _documentReader.Reader is not null)
            return new Dictionary<string, int> { { "Total", _documentReader.Reader.NumDocs } };

        return _documentReader.TermsCounter(request.Value.Field, request.Value.IsNumeric);
    }

    public IDictionary<string, string> GetLatestAddedItems(CounterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        _documentReader.Init();

        return _documentReader.LatestAdded(request.Field, request.AdditionalField, request.SortByField, ListSortDirection.Descending, request.Top.Value);
    }

    #endregion
}
