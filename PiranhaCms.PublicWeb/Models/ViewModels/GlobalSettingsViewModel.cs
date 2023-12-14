namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record GlobalSettingsViewModel
{
	public Guid? StartPageId { get; set; }
	public string? SiteTitle { get; set; }
	public string? PageTitle { get; set; }
	public string? LanguageCode { get; set; }
	public string? EmailAddress { get; set; }
	public string? PhoneNumber { get; set; }
}
