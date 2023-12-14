using PiranhaCMS.ContentTypes.Regions;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record HeaderViewModel
{
	public string? SiteLogoImageUrl { get; set; }
	public IList<LinkButton>? TopLinks { get; set; }
	public string? EmailAddress { get; set; }
	public string? PhoneNumber { get; set; }
}
