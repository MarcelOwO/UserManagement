namespace UserManagement.Core;

public class PaginatedResult<T>
{
    public required List<T> Items { get; set; }
    public required int TotalCount { get; set; }
    public required int Page { get; set; }
    public required int PageSize { get; set; }

    public bool HasNextPage => Page * PageSize > TotalCount;
    public bool HasPreviousPage => Page > 1;
}