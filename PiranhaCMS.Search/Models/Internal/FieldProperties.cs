using PiranhaCMS.Search.Models.Enums;
using System.Reflection;

namespace PiranhaCMS.Search.Models.Internal;

public record FieldProperties(
    string FieldName,
    FieldTypeEnum FieldType,
    bool Stored,
    bool IsFacet,
    bool IsArray,
    PropertyInfo OriginalProperty);
