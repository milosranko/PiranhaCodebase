namespace PiranhaCMS.Search.Models;

public struct PaginationRequest
{
    public PaginationRequest(int pageSize, int pageIndex, string queryString = "")
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        QueryString = queryString;
    }

    public int PageSize { get; private set; }
    public int PageIndex { get; private set; }
    public string QueryString { get; private set; }
}
