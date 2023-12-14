namespace PiranhaCMS.Search.Models;

public record SearchRequest
{
	public string Text { get; set; }
	public Pagination Pagination { get; set; }
}