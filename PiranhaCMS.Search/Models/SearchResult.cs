namespace PiranhaCMS.Search.Models;

public record SearchResult
{
    public static SearchResult Empty => new()
    {
        Hits = [],
        SearchText = string.Empty,
        TotalHits = 0,
        Pagination = new Pagination(0, 0)
    };

    public string SearchText { get; init; }
    public int TotalHits { get; set; }
    public IEnumerable<SearchHit> Hits { get; set; }
    public bool HasHits => Hits != null && Hits.Any();
    public Pagination Pagination { get; set; }
}