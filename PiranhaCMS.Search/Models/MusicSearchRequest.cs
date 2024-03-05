using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Models;

public record MusicSearchRequest
{
    public string Text { get; set; }
    public string[] Terms { get; set; }
    public Pagination Pagination { get; set; }
    public QueryTypesEnum QueryType { get; set; }
    public string[] Fields { get; set; }
}