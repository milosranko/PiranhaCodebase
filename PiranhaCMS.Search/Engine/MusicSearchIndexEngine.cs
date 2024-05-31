using PiranhaCMS.Search.Extensions;
using PiranhaCMS.Search.Helpers;
using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using PiranhaCMS.Search.Providers;
using PiranhaCMS.Search.Startup;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

public class MusicSearchIndexEngine<T> : ISearchIndexEngine<T> where T : MappingDocumentBase<T>, IDocument, new()
{
    #region Private fields

    private readonly Lucene.Net.Store.Directory _directory;
    private readonly Lucene.Net.Store.Directory _taxoDirectory;

    #endregion

    #region Constructors

    public MusicSearchIndexEngine(PiranhaSearchServiceBuilder serviceBuilder)
    {
        _directory = DirectoryProvider.CreateDocumentIndex(serviceBuilder);
        _taxoDirectory = DirectoryProvider.CreateFacetIndex(serviceBuilder);

        DocumentModelHelpers<T>.ReflectDocumentFields(serviceBuilder.IndexDirectory);
    }

    #endregion

    #region Public methods

    public bool IndexNotExistsOrEmpty()
    {
        using var dr = new DocumentReader(_directory, _taxoDirectory, DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
        return dr.IndexNotExistsOrEmpty();
    }

    public SearchResultDto<T> Search(SearchRequestInternal request)
    {
        using var dr = new DocumentReader(_directory, _taxoDirectory, DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);

        //TODO Extend SearchRequest, discover field type for each term, each search term should have: field name, field value, field type, search type
        //Extend facets search to accept different query than search query

        return dr.Search(request).ToDto<T>();
    }

    public IDictionary<string, int> CountDocuments(CounterRequest? request)
    {
        using var dr = new DocumentReader(_directory, _taxoDirectory, DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
        if (request is null && dr.Reader is not null)
            return new Dictionary<string, int> { { "Total", dr.Reader.NumDocs } };

        return dr.TermsCounter(request.Value.Field, request.Value.IsNumeric);
    }

    public IDictionary<string, string> GetLatestAddedItems(CounterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var dr = new DocumentReader(_directory, _taxoDirectory, DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);

        return dr.LatestAdded(request.Field, request.AdditionalField, request.SortByField, ListSortDirection.Descending, request.Top.Value);
    }

    #endregion
}
