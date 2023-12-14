using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Piranha.Extend.Fields;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace PiranhaCMS.PublicWeb.Helpers;

public static class HtmlHelpers
{
	#region Image tag

	public static IHtmlContent RenderImage<TModel, TResult>(
		this IHtmlHelper<TModel> model,
		Expression<Func<TModel, TResult>> expression,
		object htmlAttributes)
	{
		var expressionProvider = new ModelExpressionProvider(model.MetadataProvider);
		var metadata = expressionProvider.CreateModelExpression(model.ViewData, expression);
		var hasValue = metadata.Model != null;

		if (hasValue)
		{
			RouteValueDictionary? attributes = null;
			var tagBuilder = new TagBuilder("img");

			if (htmlAttributes != null)
			{
				attributes = new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
			}

			if (metadata.ModelExplorer.ModelType == typeof(ImageField) && ((ImageField)metadata.Model).HasValue)
			{
				tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
				SetImageAttributes((ImageField)metadata.Model, ref attributes);

				tagBuilder.MergeAttributes(attributes);
			}

			return tagBuilder.ToHtmlString();
		}

		return HtmlString.Empty;
	}

	private static void SetImageAttributes(
		ImageField imageRef,
		ref RouteValueDictionary attributes)
	{
		var media = imageRef.Media;
		var imageAlt = media.AltText ?? string.Empty;
		var imageUrl = media.PublicUrl.Remove(0, 1);
		var src = imageUrl;

		if (attributes["srcset"] != null)
		{
			var srcSet = (string)attributes["srcset"];
			src = imageUrl.GetSrc(srcSet.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).First(), (string)attributes["mode"]);

			if (attributes["sizes"] != null)
			{
				attributes["srcset"] = imageUrl.GetSrcSet(srcSet, (string)attributes["mode"]);
			}
			else
			{
				attributes.Remove("srcset");
			}
		}

		attributes.Add("src", src);
		attributes.Add("alt", imageAlt);

		if (attributes["mode"] != null)
		{
			attributes.Remove("mode");
		}
	}

	private static string GetSrc(this string imageUrl, string srcSet = null, string mode = null)
	{
		var result = new StringBuilder();

		if (srcSet == null)
			return imageUrl;

		var dim = srcSet.Split('x');
		var width = dim[0];
		var height = string.Empty;

		if (dim.Length == 2)
		{
			height = "&h=" + dim[1];
		}

		return result
			.AppendFormat($"{imageUrl}?w={width}{height}{(mode != null ? "&mode=" + mode : string.Empty)}")
			.ToString();
	}

	private static string GetSrcSet(this string imageUrl, string srcSet = null, string mode = null)
	{
		if (srcSet == null)
			return imageUrl;

		var breakingPoints = srcSet.Split('|');
		var result = new StringBuilder();

		foreach (var point in breakingPoints)
		{
			var dim = point.Split('x');
			var width = dim[0];
			var height = string.Empty;

			if (dim.Length == 2)
			{
				height = "&h=" + dim[1];
			}

			result.AppendFormat($"{imageUrl}?w={width}{height}{(mode != null ? "&mode=" + mode : string.Empty)} {width}w, ");
		}

		return result.ToString().TrimEnd(' ', ',');
	}

	#endregion

	private static HtmlString ToHtmlString(this TagBuilder tagBuilder)
	{
		using (var writer = new StringWriter())
		{
			tagBuilder.WriteTo(writer, HtmlEncoder.Default);
			return new HtmlString(HttpUtility.HtmlDecode(writer.ToString()));
		}
	}
}
