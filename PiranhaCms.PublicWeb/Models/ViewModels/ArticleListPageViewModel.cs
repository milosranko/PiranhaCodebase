using PiranhaCMS.Common.Extensions;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.PublicWeb.Models.ViewModels.Base;

namespace PiranhaCMS.PublicWeb.Models.ViewModels;

public record ArticleListPageViewModel : PageViewModel<ArticleListPage>
{
	public IEnumerable<ArticleListItem> Articles { get; set; }

	public ArticleListPageViewModel(ArticleListPage currentPage) : base(currentPage)
	{
		Articles = currentPage.GetChildrenPages()
			.AsPage<ArticlePage>()
			.Where(x => x.IsPublished)
			.OrderByDescending(x => x.Published)
			.Select(x => new ArticleListItem
			{
				ImageUrl = x.PrimaryImage?.Media?.PublicUrl,
				PublishedDate = x.Published.Value,
				Title = x.Title,
				Link = x.Permalink
			});
	}
}

public record ArticleListItem
{
	public string? ImageUrl { get; set; }
	public DateTime PublishedDate { get; set; }
	//TODO
	//public string[] Categories { get; set; } 
	public string? Title { get; set; }
	public string? Link { get; set; }
}
