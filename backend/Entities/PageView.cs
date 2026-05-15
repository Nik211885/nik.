namespace backend.Entities;

/// <summary>
/// Records a single page-view event sent by the public frontend on every route change.
/// </summary>
public class PageView : BaseEntity
{
    /// <summary>Frontend route path that was visited (e.g. <c>/travel</c>).</summary>
    public string Path { get; set; }

    /// <summary>Client IP address extracted from the request.</summary>
    public string IpAddress { get; set; }

    /// <summary>Raw User-Agent string from the request header.</summary>
    public string UserAgent { get; set; }

    /// <summary>Parsed browser name (e.g. <c>Chrome</c>, <c>Firefox</c>).</summary>
    public string Browser { get; set; }

    /// <summary>Parsed operating system name (e.g. <c>Windows 10</c>, <c>macOS</c>).</summary>
    public string Os { get; set; }

    /// <summary>HTTP Referer header value, or <see langword="null"/> when absent.</summary>
    public string? Referer { get; set; }

    /// <summary>UTC timestamp when the page view was recorded.</summary>
    public DateTimeOffset CreatedDate { get; set; }
}
