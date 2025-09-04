namespace PiranhaCMS.Search.Models;

public struct PaginationRequest
{
    public PaginationRequest(uint pageSize, uint pageIndex, string queryString = "")
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        QueryString = queryString;
    }

    public uint PageSize { get; private set; }
    public uint PageIndex { get; private set; }
    public string QueryString { get; private set; }
}
