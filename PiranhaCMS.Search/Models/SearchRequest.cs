namespace PiranhaCMS.Search.Models;

public class SearchRequest
{
    public string Text { get; set; }
    public Pagination Pagination { get; set; }
}