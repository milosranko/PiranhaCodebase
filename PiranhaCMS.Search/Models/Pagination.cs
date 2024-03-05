namespace PiranhaCMS.Search.Models;

public record Pagination
{
    public Pagination(int pageSize, int pageIndex)
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
    }

    public int PageSize { get; private set; }
    public int PageIndex { get; private set; }
}