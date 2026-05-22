using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Linq;

namespace backend.Controllers;

/// <summary>Generates the sitemap.xml for public pages, used by search engine crawlers.</summary>
[ApiController]
[Route("sitemap.xml")]
public class SitemapController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private const string SiteUrl = "https://nikstory.io.vn";

    /// <summary>Initialises the controller with required dependencies.</summary>
    public SitemapController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns the XML sitemap containing all public URLs.</summary>
    /// <returns>XML sitemap document.</returns>
    [HttpGet]
    [ResponseCache(Duration = 3600)]
    public async Task<ContentResult> GetSitemap()
    {
        var articles = await _db.Articles
            .AsNoTracking()
            .Select(a => new { a.Slug, a.UpdatedDate })
            .ToListAsync();

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var staticUrls = new[]
        {
            (Loc: $"{SiteUrl}/",            LastMod: DateTime.UtcNow, ChangeFreq: "daily",   Priority: "1.0"),
            (Loc: $"{SiteUrl}/photography", LastMod: DateTime.UtcNow, ChangeFreq: "weekly",  Priority: "0.8"),
            (Loc: $"{SiteUrl}/travel",      LastMod: DateTime.UtcNow, ChangeFreq: "weekly",  Priority: "0.8"),
            (Loc: $"{SiteUrl}/fashion",     LastMod: DateTime.UtcNow, ChangeFreq: "weekly",  Priority: "0.8"),
            (Loc: $"{SiteUrl}/about",       LastMod: DateTime.UtcNow, ChangeFreq: "monthly", Priority: "0.6"),
            (Loc: $"{SiteUrl}/contact",     LastMod: DateTime.UtcNow, ChangeFreq: "yearly",  Priority: "0.4"),
        };

        var urlElements = staticUrls.Select(u => new XElement(ns + "url",
            new XElement(ns + "loc", u.Loc),
            new XElement(ns + "lastmod", u.LastMod.ToString("yyyy-MM-dd")),
            new XElement(ns + "changefreq", u.ChangeFreq),
            new XElement(ns + "priority", u.Priority)
        ));

        var articleElements = articles.Select(a => new XElement(ns + "url",
            new XElement(ns + "loc", $"{SiteUrl}/post/{a.Slug}"),
            new XElement(ns + "lastmod", a.UpdatedDate.ToString("yyyy-MM-dd")),
            new XElement(ns + "changefreq", "monthly"),
            new XElement(ns + "priority", "0.9")
        ));

        var sitemap = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "urlset", urlElements.Concat(articleElements))
        );

        return Content(sitemap.ToString(SaveOptions.None), "application/xml", Encoding.UTF8);
    }
}
