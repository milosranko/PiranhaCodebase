using Piranha.AttributeBuilder;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Models.Pages.Base;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.PublicWeb.Models.Pages;

[PageType(Title = "Start Page", UseBlocks = true)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(StartPage)}")]
[AllowedPageTypes(Availability.None)]
public class StartPage : Page<StartPage>, IPage
{ }
