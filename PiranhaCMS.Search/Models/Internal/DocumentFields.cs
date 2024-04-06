using Lucene.Net.Facet;
using PiranhaCMS.Search.Models.Base;

namespace PiranhaCMS.Search.Models.Internal;

public static class DocumentFields<T> where T : IDocument
{
    private static string _indexName { get; set; } = string.Empty;
    private static IDictionary<string, FieldProperties> _fields { get; set; }
    private static FacetsConfig _facetsConfig { get; set; }

    public static string IndexName => _indexName;
    public static FacetsConfig FacetsConfig => _facetsConfig;

    public static bool HasFacets => _fields.Any(x => x.Value.IsFacet);
    public static bool HasFields => _fields.Any();

    public static void SetFields(string indexName, IDictionary<string, FieldProperties> fields, FacetsConfig facetsConfig)
    {
        _indexName = indexName;
        _fields = fields;
        _facetsConfig = facetsConfig;
    }

    public static FieldProperties? GetField(string fieldName)
    {
        if (_fields.ContainsKey(fieldName))
            return _fields[fieldName];

        return _fields.SingleOrDefault(x => x.Value.FieldName.Equals(fieldName)).Value;
    }

    public static IDictionary<string, FieldProperties> AllFields => _fields;
}
