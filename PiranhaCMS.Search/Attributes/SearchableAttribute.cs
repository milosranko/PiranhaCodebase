using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SearchableAttribute : Attribute
{
    public FieldTypeEnum FieldType { get; set; }
    public bool Stored { get; set; } = true;
    public string? FieldName { get; set; }
}
