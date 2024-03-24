using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Facets;

namespace PiranhaCMS.Search.Models.Dto;

public struct SearchResultDto<T> where T : IDocument
{
    public static SearchResultDto<T> Empty()
    {
        return new()
        {
            Hits = [],
            Text = string.Empty,
            TotalHits = 0,
            Pagination = new PaginationDto(0, 0, 0)
        };
    }

    public string Text { get; set; }
    public IDictionary<string, string?>? SearchFields { get; set; }
    public int TotalHits { get; set; }
    public IEnumerable<T> Hits { get; set; }
    public readonly bool HasHits => Hits != null && Hits.Any();
    public PaginationDto Pagination { get; set; }
    public IEnumerable<FacetFilter>? Facets { get; set; }
}
