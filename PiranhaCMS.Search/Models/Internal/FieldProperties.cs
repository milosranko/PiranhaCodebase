using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Models.Internal;

public struct FieldProperties
{
    public required string FieldName { get; set; }
    public required FieldTypeEnum FieldType { get; set; }
    public required bool Stored { get; set; }
    public required bool IsFacet { get; set; }
    public required bool IsArray { get; set; }
}
