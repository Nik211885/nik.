using backend.Data;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.PageView.Requests;
using backend.ViewModels.PageView.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for page-view tracking and statistics.</summary>
public class PageViewServices
{
    private readonly ILogger<PageViewServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public PageViewServices(
        ILogger<PageViewServices> logger,
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpContext = httpContext;
    }

    /// <summary>Records a page-view event from the public frontend.</summary>
    /// <param name="request">The path and optional referer.</param>
    public async Task RecordAsync(CreatePageViewRequest request)
    {
        var ctx = _httpContext.HttpContext!;
        var ip = GetClientIp(ctx);
        var ua = ctx.Request.Headers.UserAgent.ToString();

        _dbContext.PageViews.Add(new Entities.PageView
        {
            Path = request.Path.Trim(),
            IpAddress = ip,
            UserAgent = ua.Length > 1000 ? ua[..1000] : ua,
            Browser = ParseBrowser(ua),
            Os = ParseOs(ua),
            Referer = string.IsNullOrWhiteSpace(request.Referer) ? null : request.Referer.Trim(),
            CreatedDate = DateTimeOffset.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>Returns a paginated list of page-view records, newest first.</summary>
    /// <param name="request">Pagination parameters.</param>
    public async Task<PaginationItem<PageViewResponse>> GetPaginationAsync(PaginationRequest request)
    {
        return await _dbContext.PageViews
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedDate)
            .Select(p => new PageViewResponse
            {
                Id = p.Id,
                Path = p.Path,
                IpAddress = p.IpAddress,
                Browser = p.Browser,
                Os = p.Os,
                Referer = p.Referer,
                CreatedDate = p.CreatedDate
            })
            .PaginationItemAsync(request);
    }

    /// <summary>
    /// Returns aggregated statistics for the chart and KPI cards.
    /// </summary>
    /// <param name="period"><c>week</c> = last 7 days, <c>month</c> = last 30 days, <c>year</c> = last 12 months.</param>
    public async Task<PageViewStatsResponse> GetStatsAsync(string period)
    {
        var now = DateTimeOffset.UtcNow;
        DateTimeOffset from;

        from = period switch
        {
            "year" => now.AddMonths(-12),
            "month" => now.AddDays(-30),
            _ => now.AddDays(-7)
        };

        var rows = await _dbContext.PageViews
            .AsNoTracking()
            .Where(p => p.CreatedDate >= from)
            .Select(p => new { p.CreatedDate, p.IpAddress, p.Path, p.Browser, p.Os })
            .ToListAsync();

        // KPIs
        var totalViews = rows.Count;
        var uniqueIps = rows.Select(r => r.IpAddress).Distinct().Count();

        // Chart grouping
        List<ChartDataPoint> chart;

        if (period == "year")
        {
            chart = rows
                .GroupBy(r => new { r.CreatedDate.Year, r.CreatedDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new ChartDataPoint
                {
                    Label = $"T{g.Key.Month:D2}/{g.Key.Year}",
                    Views = g.Count(),
                    UniqueIps = g.Select(x => x.IpAddress).Distinct().Count()
                })
                .ToList();
        }
        else
        {
            chart = rows
                .GroupBy(r => r.CreatedDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key.ToString("dd/MM"),
                    Views = g.Count(),
                    UniqueIps = g.Select(x => x.IpAddress).Distinct().Count()
                })
                .ToList();
        }

        // Top pages
        var topPages = rows
            .GroupBy(r => r.Path)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new TopPage { Path = g.Key, Count = g.Count() })
            .ToList();

        // Browser breakdown
        var browsers = rows
            .GroupBy(r => r.Browser)
            .OrderByDescending(g => g.Count())
            .Select(g => new PieSlice { Label = g.Key, Count = g.Count() })
            .ToList();

        // OS breakdown
        var operatingSystems = rows
            .GroupBy(r => r.Os)
            .OrderByDescending(g => g.Count())
            .Select(g => new PieSlice { Label = g.Key, Count = g.Count() })
            .ToList();

        // Hourly distribution (aggregate across all days in period)
        var hourlyDistribution = Enumerable.Range(0, 24)
            .Select(h => new HourlyPoint
            {
                Hour = h,
                Views = rows.Count(r => r.CreatedDate.ToLocalTime().Hour == h)
            })
            .ToList();

        return new PageViewStatsResponse
        {
            Chart = chart,
            TotalViews = totalViews,
            UniqueIps = uniqueIps,
            TopPages = topPages,
            Browsers = browsers,
            OperatingSystems = operatingSystems,
            HourlyDistribution = hourlyDistribution
        };
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static string GetClientIp(HttpContext ctx)
    {
        var forwarded = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded))
            return forwarded.Split(',')[0].Trim();

        return ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string ParseBrowser(string ua)
    {
        if (ua.Contains("Edg/")) return "Microsoft Edge";
        if (ua.Contains("OPR/") || ua.Contains("Opera")) return "Opera";
        if (ua.Contains("Chrome/")) return "Chrome";
        if (ua.Contains("Firefox/")) return "Firefox";
        if (ua.Contains("Safari/") && !ua.Contains("Chrome")) return "Safari";
        return "Other";
    }

    private static string ParseOs(string ua)
    {
        if (ua.Contains("Windows NT 10")) return "Windows 10/11";
        if (ua.Contains("Windows")) return "Windows";
        if (ua.Contains("Mac OS X")) return "macOS";
        if (ua.Contains("Android")) return "Android";
        if (ua.Contains("iPhone") || ua.Contains("iPad")) return "iOS";
        if (ua.Contains("Linux")) return "Linux";
        return "Other";
    }
}
