namespace backend.ViewModels;

/// <summary>
/// Standard input model for paginated list endpoints.
/// Pass as <c>[AsParameters]</c> on GET actions.
/// </summary>
public class PaginationRequest
{
    /// <summary>One-based page number. Defaults to 1 when &lt;= 0.</summary>
    public int PageNumber { get; set; }

    /// <summary>Number of items per page. Defaults to 10 when &lt;= 0.</summary>
    public int PageSize { get; set; }
}
