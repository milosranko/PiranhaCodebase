using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Models;
using PiranhaCMS.Common;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Helpers;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Config;
using System.Text;

namespace PiranhaCMS.Search.Services;

public class SearchIndexService : ISearch
{
	private readonly ISearchIndexEngine _engine;

	public SearchIndexService(ISearchIndexEngine engine)
	{
		_engine = engine;
	}

	public Task SavePageAsync(PageBase page)
	{
		if (!SearchOptions.Include.Any(x => x.Name.Equals(page.TypeId, StringComparison.InvariantCultureIgnoreCase)))
			return Task.CompletedTask;

		if (!page.Published.HasValue || page.Permissions.Any() || !(page is DynamicPage))
		{
			return Task.Run(() =>
			{
				_engine.DeleteById(page.Id.ToString());
			});
		}

		using var serviceScope = ServiceActivator.GetScope();
		var api = serviceScope.ServiceProvider.GetRequiredService<IApi>();
		var site = api.Sites.GetByIdAsync(page.SiteId).GetAwaiter().GetResult();
		var dynamicPage = (DynamicPage)page;
		var doc = new Content
		{
			RouteName = page.Slug,
			ContentId = page.Id.ToString(),
			ContentType = page.TypeId,
			Title = page.Title,
			Text = PageContentHelpers.ExtractPageContent(dynamicPage),
			Category = page.TypeId,
			Url = page.Permalink,
			Culture = site.LanguageId.ToString()
		};

		return Task.Run(() =>
		{
			_engine.AddToIndex(doc);
		});
	}

	public Task DeletePageAsync(PageBase page)
	{
		return Task.Run(() =>
		{
			_engine.DeleteById(page.Id.ToString());
		});
	}

	public Task SavePostAsync(PostBase post)
	{
		if (!post.Published.HasValue || post.Permissions.Any())
		{
			return Task.Run(() =>
			{
				_engine.DeleteById(post.Id.ToString());
			});
		}

		var body = new StringBuilder();
		PageContentHelpers.ExtractBlocksContent(post.Blocks, ref body);

		var doc = new Content
		{
			RouteName = post.Slug,
			ContentId = post.Id.ToString(),
			ContentType = post.TypeId,
			Title = post.Title,
			Text = PageContentHelpers.CleanContent(body),
			Category = string.Empty,
			Url = post.Permalink,
			Culture = string.Empty
		};

		return Task.Run(() =>
		{
			_engine.AddToIndex(doc);
		});
	}

	public Task DeletePostAsync(PostBase post)
	{
		return Task.Run(() =>
		{
			_engine.DeleteById(post.Id.ToString());
		});
	}
}
