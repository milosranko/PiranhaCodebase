namespace PiranhaCMS.Search.Models.Facets;

public record FacetValue
{
    public string? Value { get; set; }
    public int Count { get; set; }
    public string? QueryString { get; set; }
}
