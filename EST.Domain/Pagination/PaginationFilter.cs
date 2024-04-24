namespace EST.Domain.Pagination;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortColumn { get; set; }
    public string SortDirection { get; set; }
    public string? TypeItemsVisibility { get; set; }
    public string? Filter { get; set; }
    public string? SearchCategory { get; set; }

    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 5;
        SortColumn = "name";
        SortDirection = "asc";
        TypeItemsVisibility = "all";
        Filter = "";
        SearchCategory = "";
    }

    public PaginationFilter(int pageNumber, int pageSize, string sortColumn, string sortDirection)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 5 ? 5 : pageSize;
        SortColumn = sortColumn;
        SortDirection = sortDirection;
    }
}