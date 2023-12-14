using PiranhaCMS.ContentTypes.Pages.Base;
using System.Text.Json.Serialization;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record PageViewModel<T> : IPageViewModel<T> where T : IPage
{
	public PageViewModel(T currentPage)
	{
		CurrentPage = currentPage;
		Header = new HeaderViewModel();
		Footer = new FooterViewModel();
		GlobalSettings = new GlobalSettingsViewModel();
	}

	[JsonIgnore]
	public T CurrentPage { get; }
	public HeaderViewModel Header { get; set; }
	public FooterViewModel Footer { get; set; }
	public GlobalSettingsViewModel GlobalSettings { get; set; }
}
