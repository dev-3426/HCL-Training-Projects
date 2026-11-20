namespace Enrollora.Api.DTOs;

public sealed record ApiMessage(string Message);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalRecords,
    int TotalPages);

public  class PaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";

    public void Normalize()
    {
        PageNumber = Math.Max(1, PageNumber);
        PageSize = Math.Clamp(PageSize, 1, 100);
        SortOrder = SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? "desc"
            : "asc";
    }
}
