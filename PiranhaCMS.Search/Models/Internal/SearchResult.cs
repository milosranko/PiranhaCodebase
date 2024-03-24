using Lucene.Net.Documents;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Facets;

namespace PiranhaCMS.Search.Models.Internal;

internal struct SearchResult
{
    public static SearchResult Empty => new()
    {
        Hits = [],
        SearchParam = string.Empty,
        SearchText = string.Empty,
        TotalHits = 0,
        Pagination = new PaginationDto(0, 0, 0)
    };

    public string SearchParam { get; set; }
    public string SearchText { get; set; }
    public int TotalHits { get; set; }
    public IEnumerable<Document> Hits { get; set; }
    public readonly bool HasHits => Hits != null && Hits.Any();
    public PaginationDto Pagination { get; set; }
    public IEnumerable<FacetFilter> Facets { get; set; }
}
