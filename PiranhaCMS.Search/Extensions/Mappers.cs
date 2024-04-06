using Lucene.Net.Facet;
using PiranhaCMS.Common.Extensions;
using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Facets;
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
            Hits = searchResult.Hits
            .AsParallel()
            .Select(x => new T().MapFromLuceneDocument(x))
            .ToArray()
        };
    }

    public static FacetFilter ToFacetFilter(this FacetResult facet, string queryString)
    {
        return new FacetFilter
        {
            Name = facet.Dim,
            Values = facet.LabelValues
            .Select(x =>
            new FacetValue
            {
                Value = x.Label,
                Count = (int)x.Value,
                QueryString = facet.Dim.Equals("art")
                ? queryString.RemoveQueryStringParameter("rel").AddOrReplaceQueryStringParameter(facet.Dim, x.Label)
                : queryString.AddOrReplaceQueryStringParameter(facet.Dim, x.Label)
            })
            .ToArray()
        };
    }
}
