namespace backend.ViewModels;
public class PaginationItem<T>
{
    public IReadOnlyCollection<T> Data { get; set; }
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
    public PaginationItem(int pageNumber, int pageSize, int totalItem, IReadOnlyCollection<T> data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItem;
        PageCount = (int)Math.Ceiling((double)totalItem / pageSize);
        Data = data;
    }
}
