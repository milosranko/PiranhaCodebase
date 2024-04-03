using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Models.Internal;

public struct SearchField
{
    public string Name { get; set; }
    public string Value { get; set; }
    public FieldProperties Properties { get; set; }
    public SearchType SearchType { get; set; }
}
