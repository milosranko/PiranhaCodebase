using PiranhaCMS.Common.Extensions;
using PiranhaCMS.ContentTypes.Pages;
using System.Collections.Generic;
using System.Linq;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public class ArticlePageViewModel : PageViewModel<ArticlePage>
{
    public IEnumerable<SubMenuItem> SubMenu { get; set; }

    public ArticlePageViewModel(ArticlePage currentPage) : base(currentPage)
    {
        SubMenu = currentPage.GetChildrenPages()
            .Select(x => new SubMenuItem
            {
                Name = x.Title,
                Link = x.Permalink
            });
    }
}

public struct SubMenuItem
{
    public string Name { get; set; }
    public string Link { get; set; }
}
