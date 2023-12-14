namespace PiranhaCMS.Search.Models;

public record Content : WebPage
{
	public string ContentId { get; set; }
	public string RouteName { get; set; }
	public string ContentType { get; set; }
	public string Category { get; set; }
	public string Culture { get; set; }
	//public IList<string> Tags { get; set; } = new List<string>();
}
