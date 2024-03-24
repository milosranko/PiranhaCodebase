using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Internal;

namespace PiranhaCMS.Search.Extensions;

internal static class Mappers
{
    public static SearchResultDto<T> ToDto<T>(this SearchResult searchResult) where T : MappingDocumentBase<T>, IDocument, new()
    {
        return new SearchResultDto<T>
        {
            Text = searchResult.SearchText,
            TotalHits = searchResult.TotalHits,
            Pagination = searchResult.Pagination,
            Facets = searchResult.Facets,
            Hits = searchResult.Hits.Select(x => new T().MapFromLuceneDocument(x))
        };
    }
}
