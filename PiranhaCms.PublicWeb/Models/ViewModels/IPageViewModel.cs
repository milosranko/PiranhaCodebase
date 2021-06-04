using PiranhaCMS.PublicWeb.Models.Pages.Base;
using System;

namespace PiranhaCMS.PublicWeb.Models.ViewModels
{
    public interface IPageViewModel<out T> where T : IPage
    {
        T CurrentPage { get; }
        string PageTitle { get; set; }
        string LanguageCode { get; set; }
        Guid StartPageId { get; set; }
        HeaderViewModel Header { get; set; }
        FooterViewModel Footer { get; set; }
    }
}
