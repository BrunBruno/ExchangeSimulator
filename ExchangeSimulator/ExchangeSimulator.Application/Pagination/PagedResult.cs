namespace ExchangeSimulator.Application.Pagination;

public class PagedResult<T> where T : class
{
    public List<T> Items { get; set; }
    public int TotalPages { get; set; }
    public int ItemsFrom { get; set; }
    public int ItemsTo { get; set; }
    public int TotalItemsCount { get; set; }

    public PagedResult(List<T> items, int totalCount, int pageSize, int pageNumber)
    {
        Items = items
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        TotalItemsCount = totalCount;
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = pageSize * (pageNumber - 1) + pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}