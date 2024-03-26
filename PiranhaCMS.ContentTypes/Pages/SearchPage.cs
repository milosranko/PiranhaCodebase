using Piranha.AttributeBuilder;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Pages.Base;
using PiranhaCMS.Validators.Attributes;

namespace PiranhaCMS.ContentTypes.Pages;

[PageType(Title = "Search Page", UseBlocks = false)]
[ContentTypeRoute(Title = "Default", Route = $"/{nameof(SearchPage)}")]
[AllowedPageTypes([
    typeof(MusicSearchPage)
])]
public class SearchPage : Page<SearchPage>, IPage
{ }
