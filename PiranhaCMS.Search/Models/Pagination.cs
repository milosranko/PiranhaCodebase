namespace PiranhaCMS.Search.Models;

public record Pagination
{
    public Pagination(int pageSize, int pageIndex, string queryString = "")
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        QueryString = queryString;
    }

    public int PageSize { get; private set; }
    public int PageIndex { get; private set; }
    public string QueryString { get; private set; }
}