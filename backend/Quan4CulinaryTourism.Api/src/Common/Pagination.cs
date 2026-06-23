namespace Quan4CulinaryTourism.Api.Common;

public class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalItems { get; set; }
    public int TotalPages { get; set; }
}
