using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Models.Internal;

public struct SearchRequestInternal
{
    public PaginationRequest Pagination { get; set; }
    public QueryTypesEnum QueryType { get; set; }
    public IEnumerable<SearchField> SearchFields { get; set; }
    public IDictionary<string, IEnumerable<string?>?>? Facets { get; set; }
}
