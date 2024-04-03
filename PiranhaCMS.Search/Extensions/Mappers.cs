using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Internal;

namespace PiranhaCMS.Search.Extensions;

internal static class Mappers
{
    public static SearchResultDto<T> ToDto<T>(this SearchResultInternal searchResult) where T : MappingDocumentBase<T>, IDocument, new()
    {
        return new SearchResultDto<T>
        {
            SearchRequest = searchResult.SearchRequest,
            Text = searchResult.SearchText,
            TotalHits = searchResult.TotalHits,
            Pagination = searchResult.Pagination,
            Facets = searchResult.Facets,
            Hits = searchResult.Hits.Select(x => new T().MapFromLuceneDocument(x))
        };
    }

    //public static SearchRequestInternal ToInternal(this Models.Requests.SearchRequest request)
    //{
    //    return new SearchRequestInternal
    //    {
    //        SearchFields = request.SearchFields.Select(x => new SearchField
    //        {
    //            Name = x.Key,
    //            Value = x.Value,
    //            Properties = DocumentFields<MusicLibraryDocument>.GetField(x.Key).Value,
    //            SearchType = request.SearchType
    //        }),
    //        QueryType = request.QueryType,
    //        Pagination = request.Pagination,
    //        Facets = request.Facets
    //    };
    //}
}
