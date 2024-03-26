﻿using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Models.Requests;

public struct SearchRequest
{
    public string? Text { get; set; }
    public PaginationRequest Pagination { get; set; }
    public QueryTypesEnum QueryType { get; set; }
    public IDictionary<string, string?>? SearchFields { get; set; }
    public SearchType SearchType { get; set; }
    public IDictionary<string, IEnumerable<string?>?>? Facets { get; set; }
}