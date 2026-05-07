namespace backend.ViewModels;

/// <summary>
/// Generic paginated response wrapper returned by all list endpoints
/// that support paging.
/// </summary>
/// <typeparam name="T">The item type contained in this page.</typeparam>
public class PaginationItem<T>
{
    /// <summary>Items on the current page.</summary>
    public IReadOnlyCollection<T> Data { get; set; }

    /// <summary>Total number of items across all pages.</summary>
    public int TotalItems { get; set; }

    /// <summary>Current one-based page number.</summary>
    public int PageNumber { get; set; }

    /// <summary>Maximum number of items per page.</summary>
    public int PageSize { get; set; }

    /// <summary>Total number of pages computed from <see cref="TotalItems"/> and <see cref="PageSize"/>.</summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Initialises a paginated result.
    /// </summary>
    /// <param name="pageNumber">Current page number (1-based).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="totalItem">Total item count across all pages.</param>
    /// <param name="data">Items on the current page.</param>
    public PaginationItem(int pageNumber, int pageSize, int totalItem, IReadOnlyCollection<T> data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItem;
        PageCount = (int)Math.Ceiling((double)totalItem / pageSize);
        Data = data;
    }
}
