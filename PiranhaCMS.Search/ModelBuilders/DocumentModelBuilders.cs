using Lucene.Net.Documents;
using Lucene.Net.Facet;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;

namespace PiranhaCMS.Search.ModelBuilders;

internal class DocumentModelBuilders
{
	public static Document BuildContentDocument(Content content)
	{
		return
		[
			new StringField(FieldNames.Id, content.ContentId, Field.Store.YES),
			new StringField(FieldNames.Url, content.Url ?? "", Field.Store.YES),
			new StringField(FieldNames.RouteName, content.RouteName, Field.Store.NO),
			new StringField(FieldNames.Type, content.ContentType, Field.Store.NO),
			new StringField(FieldNames.Category, content.Category ?? "", Field.Store.NO),
			new TextField(FieldNames.Title, content.Title ?? "", Field.Store.YES),
			new TextField(FieldNames.Text, content.Text ?? "", Field.Store.YES)
		];
	}

	public static Document BuildWebPageDocument(WebPage webPage)
	{
		return
		[
			new StringField(FieldNames.Url, webPage.Url, Field.Store.YES),
			new TextField(FieldNames.Title, webPage.Title ?? "", Field.Store.YES),
			new TextField(FieldNames.Text, webPage.Text ?? "", Field.Store.NO)
		];
	}

	public static Document BuildFacetDocument(Content content)
	{
		return
		[
			//new FacetField(FieldNames.Text, content.Text ?? ""),
			new FacetField(FieldNames.Category, content.Category)
		];
	}
}
