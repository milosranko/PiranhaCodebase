using PiranhaCMS.Common.Extensions;
using PiranhaCMS.PublicWeb.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public class ArticleListPageViewModel : PageViewModel<ArticleListPage>
{
    public IEnumerable<ArticleListItem> Articles { get; set; }

    public ArticleListPageViewModel(ArticleListPage currentPage) : base(currentPage)
    {
        Articles = currentPage.GetChildrenPages()
            .AsPage<ArticlePage>()
            .Select(x => new ArticleListItem
            {
                ImageUrl = x.PrimaryImage?.Media?.PublicUrl,
                PublishedDate = x.Published.Value,
                Title = x.Title,
                Link = x.Permalink
            });
    }
}

public struct ArticleListItem
{
    public string? ImageUrl { get; set; }
    public DateTime PublishedDate { get; set; }
    //TODO
    //public string[] Categories { get; set; } 
    public string Title { get; set; }
    public string Link { get; set; }
}
