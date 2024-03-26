﻿using PiranhaCMS.Search.Extensions;
using PiranhaCMS.Search.Helpers;
using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using System.ComponentModel;

namespace PiranhaCMS.Search.Engine;

public class MusicSearchIndexEngine<T> : ISearchIndexEngine<T> where T : MappingDocumentBase<T>, IDocument, new()
{
    //private readonly IDocumentReader _documentReader;

    public MusicSearchIndexEngine()
    {
        DocumentModelHelpers<T>.ReflectDocumentFields();

        //_documentReader = new DocumentReader(DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
    }

    #region Public methods

    public bool IndexNotExistsOrEmpty()
    {
        using var dr = new DocumentReader(DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
        return dr.IndexNotExistsOrEmpty();
    }

    public SearchResultDto<T> Search(SearchRequest request)
    {
        using var dr = new DocumentReader(DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
        return dr.Search(request).ToDto<T>();
    }

    public IDictionary<string, int> CountDocuments(CounterRequest? request)
    {
        using var dr = new DocumentReader(DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);
        if (request is null && dr.Reader is not null)
            return new Dictionary<string, int> { { "Total", dr.Reader.NumDocs } };

        return dr.TermsCounter(request.Value.Field, request.Value.IsNumeric);
    }

    public IDictionary<string, string> GetLatestAddedItems(CounterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var dr = new DocumentReader(DocumentFields<T>.IndexName, DocumentFields<T>.FacetsConfig, DocumentFields<T>.HasFacets);

        return dr.LatestAdded(request.Field, request.AdditionalField, request.SortByField, ListSortDirection.Descending, request.Top.Value);
    }

    #endregion
}
