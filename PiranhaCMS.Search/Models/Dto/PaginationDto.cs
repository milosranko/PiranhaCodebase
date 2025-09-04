namespace PiranhaCMS.Search.Models.Dto;

public struct PaginationDto
{
    public PaginationDto(uint pageSize, uint pageIndex, int totalPages, string queryString = "")
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalPages = totalPages;
        QueryString = queryString;
    }

    public uint PageSize { get; private set; }
    public uint PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public string QueryString { get; private set; } = string.Empty;
}
