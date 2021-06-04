using PiranhaCMS.PublicWeb.Models.Pages.Base;
using System;

namespace PiranhaCMS.PublicWeb.Models.ViewModels
{
    public class PageViewModel<T> : IPageViewModel<T> where T : IPage
    {
        public PageViewModel(T currentPage)
        {
            CurrentPage = currentPage;
        }

        public T CurrentPage { get; }
        public Guid StartPageId { get; set; }
        public string PageTitle { get; set; }
        public string LanguageCode { get; set; }
        public HeaderViewModel Header { get; set; }
        public FooterViewModel Footer { get; set; }
    }
}
