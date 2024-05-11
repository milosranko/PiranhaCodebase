using PiranhaCMS.ContentTypes.Pages.Base;
using System.Text.Json.Serialization;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public interface IPageViewModel<out T> where T : IPage
{
    [JsonIgnore]
    T CurrentPage { get; }
    HeaderViewModel Header { get; set; }
    FooterViewModel Footer { get; set; }
    GlobalSettingsViewModel GlobalSettings { get; set; }
}
