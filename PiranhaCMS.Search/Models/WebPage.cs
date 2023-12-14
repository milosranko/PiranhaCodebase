namespace PiranhaCMS.Search.Models;

public record WebPage
{
	public string Title { get; set; }
	public string Url { get; set; }
	public string Text { get; set; }
}