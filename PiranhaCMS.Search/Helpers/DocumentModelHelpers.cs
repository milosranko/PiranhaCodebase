using Lucene.Net.Facet;
using PiranhaCMS.Search.Attributes;
using PiranhaCMS.Search.Models.Base;
using PiranhaCMS.Search.Models.Internal;
using System.Reflection;

namespace PiranhaCMS.Search.Helpers;

internal static class DocumentModelHelpers<T> where T : IDocument
{
    public static void ReflectDocumentFields(string? indexName)
    {
        if (!string.IsNullOrEmpty(DocumentFields<T>.IndexName) && DocumentFields<T>.HasFields)
            return;

        var documentType = typeof(T);
        var index = string.IsNullOrEmpty(indexName) ? documentType.GetCustomAttribute<IndexConfigAttribute>()?.IndexName ?? "index" : indexName;
        var props = documentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var fields = new Dictionary<string, FieldProperties>(props.Length);
        var facetsConfig = new FacetsConfig();
        SearchableAttribute? searchableAttr;
        FacetPropertyAttribute? facetAttr;
        MultiValueFacetPropertyAttribute? multiValueFacetAttr;

        foreach (var prop in props)
        {
            searchableAttr = prop.GetCustomAttribute<SearchableAttribute>();
            facetAttr = prop.GetCustomAttribute<FacetPropertyAttribute>();
            multiValueFacetAttr = prop.GetCustomAttribute<MultiValueFacetPropertyAttribute>();

            if (searchableAttr is null) continue;

            var fieldName = GetFieldName(searchableAttr, prop);

            fields.Add(prop.Name, new FieldProperties(fieldName, searchableAttr.FieldType, searchableAttr.Stored, facetAttr is not null, prop.PropertyType.IsArray, prop));

            if (multiValueFacetAttr is not null)
                facetsConfig.SetMultiValued(fieldName, true);
        }

        DocumentFields<T>.SetFields(index, fields, facetsConfig);
    }

    private static string GetFieldName(SearchableAttribute? searchableAttr, PropertyInfo prop)
    {
        return string.IsNullOrEmpty(searchableAttr?.FieldName)
            ? prop.Name
            : searchableAttr.FieldName;
    }
}
