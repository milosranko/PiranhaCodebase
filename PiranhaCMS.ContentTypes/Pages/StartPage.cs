using Piranha.AttributeBuilder;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.ContentTypes.Pages;

[PageType(Title = "Start Page", UseBlocks = true, UsePrimaryImage = false, UseExcerpt = false)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(StartPage)}")]
[AllowedPageTypes(Availability.None)]
public class StartPage : Page<StartPage>, IPage
{ }
