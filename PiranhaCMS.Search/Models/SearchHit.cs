namespace PiranhaCMS.Search.Models;

public record SearchHit
{
    public string Url { get; set; }
    public string Title { get; set; }
    public float Score { get; set; }
    public string HighlightedText { get; set; }
}