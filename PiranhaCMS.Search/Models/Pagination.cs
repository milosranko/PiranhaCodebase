namespace PiranhaCMS.Search.Models;

public record Pagination
{
    public Pagination(int? pageSize, int pageIndex, string queryString = "")
    {
        PageSize = pageSize ?? PageSizeFallback;
        PageIndex = pageIndex;
        QueryString = queryString;
    }

    public const int PageSizeFallback = 5;
    public int PageSize { get; private set; }
    public int PageIndex { get; private set; }
    public int TotalPages { get; set; }
    public string QueryString { get; private set; }
}