namespace PiranhaCMS.Search.Models.Dto;

public struct PaginationDto
{
    public PaginationDto(int pageSize, int pageIndex, int totalPages, string queryString = "")
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalPages = totalPages;
        QueryString = queryString;
    }

    public int PageSize { get; private set; }
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public string QueryString { get; private set; } = string.Empty;
}
