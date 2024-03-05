using Piranha.AttributeBuilder;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.ContentTypes.Pages;

[PageType(Title = "Music Search Page", UseBlocks = true)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(MusicSearchPage)}")]
[AllowedPageTypes(Availability.None)]
public class MusicSearchPage : Page<SearchPage>, IPage
{ }
