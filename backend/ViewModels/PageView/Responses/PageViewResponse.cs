namespace backend.ViewModels.PageView.Responses;

/// <summary>Single page-view record returned for the admin table.</summary>
public class PageViewResponse
{
    /// <summary>Record ID.</summary>
    public string Id { get; set; }

    /// <summary>Visited path.</summary>
    public string Path { get; set; }

    /// <summary>Client IP address.</summary>
    public string IpAddress { get; set; }

    /// <summary>Parsed browser name.</summary>
    public string Browser { get; set; }

    /// <summary>Parsed OS name.</summary>
    public string Os { get; set; }

    /// <summary>HTTP Referer.</summary>
    public string? Referer { get; set; }

    /// <summary>UTC timestamp of the visit.</summary>
    public DateTimeOffset CreatedDate { get; set; }
}

/// <summary>Single data point in a chart series.</summary>
public class ChartDataPoint
{
    /// <summary>X-axis label (e.g. <c>15/05</c> or <c>Tháng 5</c>).</summary>
    public string Label { get; set; }

    /// <summary>Total page views for this period.</summary>
    public int Views { get; set; }

    /// <summary>Unique IP count for this period.</summary>
    public int UniqueIps { get; set; }
}

/// <summary>Aggregated statistics response for the chart and KPI cards.</summary>
public class PageViewStatsResponse
{
    /// <summary>Chart data points ordered by time ascending.</summary>
    public List<ChartDataPoint> Chart { get; set; } = [];

    /// <summary>Total views in the selected period.</summary>
    public int TotalViews { get; set; }

    /// <summary>Unique IPs in the selected period.</summary>
    public int UniqueIps { get; set; }

    /// <summary>Top 5 most visited paths.</summary>
    public List<TopPage> TopPages { get; set; } = [];

    /// <summary>View count grouped by browser name.</summary>
    public List<PieSlice> Browsers { get; set; } = [];

    /// <summary>View count grouped by operating system.</summary>
    public List<PieSlice> OperatingSystems { get; set; } = [];

    /// <summary>View count for each hour of the day (0–23) in the selected period.</summary>
    public List<HourlyPoint> HourlyDistribution { get; set; } = [];
}

/// <summary>A path and its visit count.</summary>
public class TopPage
{
    /// <summary>Visited path.</summary>
    public string Path { get; set; }

    /// <summary>Number of views.</summary>
    public int Count { get; set; }
}

/// <summary>A label/count pair used for pie and doughnut charts.</summary>
public class PieSlice
{
    /// <summary>Slice label (e.g. browser or OS name).</summary>
    public string Label { get; set; }

    /// <summary>Number of views for this slice.</summary>
    public int Count { get; set; }
}

/// <summary>Views for a single hour of the day.</summary>
public class HourlyPoint
{
    /// <summary>Hour of the day (0–23).</summary>
    public int Hour { get; set; }

    /// <summary>Number of views recorded during this hour.</summary>
    public int Views { get; set; }
}
