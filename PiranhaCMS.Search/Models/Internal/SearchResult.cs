using Lucene.Net.Documents;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Facets;

namespace PiranhaCMS.Search.Models.Internal;

internal struct SearchResultInternal
{
    public static SearchResultInternal Empty => new()
    {
        Hits = [],
        SearchParam = string.Empty,
        SearchText = string.Empty,
        TotalHits = 0,
        Pagination = new PaginationDto(0, 0, 0),
        Facets = []
    };

    public SearchRequestInternal SearchRequest { get; set; }
    public string SearchParam { get; set; }
    public string SearchText { get; set; }
    public int TotalHits { get; set; }
    public IEnumerable<Document> Hits { get; set; }
    public readonly bool HasHits => Hits != null && Hits.Any();
    public PaginationDto Pagination { get; set; }
    public FacetFilter[] Facets { get; set; }
}
