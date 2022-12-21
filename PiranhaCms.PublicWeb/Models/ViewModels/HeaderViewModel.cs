using PiranhaCMS.PublicWeb.Models.Regions;
using System.Collections.Generic;

namespace PiranhaCMS.PublicWeb.Models.ViewModels
{
    public class HeaderViewModel
    {
        public string SiteLogoImageUrl { get; set; }
        public IList<LinkButton> TopLinks { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
