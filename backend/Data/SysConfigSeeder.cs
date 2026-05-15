using System.Text.Json;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds default system-configuration entries idempotently on startup.
/// Existing keys are never overwritten so admin changes are preserved.
/// </summary>
public static class SysConfigSeeder
{
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Ensures <c>config.sidebar</c>, <c>config.social</c>, and <c>config.info</c>
    /// exist in the <c>SysConfigs</c> table. Skips any key that is already present.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var existingKeys = await db.SysConfigs
            .AsNoTracking()
            .Select(x => x.Key)
            .ToHashSetAsync();

        foreach (var (key, value) in BuildEntries())
        {
            if (!existingKeys.Contains(key))
                db.SysConfigs.Add(new SysConfig { Key = key, Value = value });
        }

        await db.SaveChangesAsync();
    }

    // ── Entry builders ────────────────────────────────────────────────────────

    private static IEnumerable<(string Key, JsonDocument Value)> BuildEntries()
    {
        yield return ("config.sidebar", Serialize(new[]
        {
            new { nameKey = "nav.home",        @ref = "/" },
            new { nameKey = "nav.photography", @ref = "/photography" },
            new { nameKey = "nav.travel",      @ref = "/travel" },
            new { nameKey = "nav.fashion",     @ref = "/fashion" },
            new { nameKey = "nav.about",       @ref = "/about" },
            new { nameKey = "nav.contact",     @ref = "/contact" },
        }));

        yield return ("config.social", Serialize(new[]
        {
            new
            {
                id = "1", name = "Facebook", @ref = "https://facebook.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#1877F2" d="M24 12a12 12 0 1 0-13.875 11.85v-8.385H7.078V12h3.047V9.356c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.926-1.956 1.874V12h3.328l-.532 3.465h-2.796v8.385A12 12 0 0 0 24 12z"/></svg>"""
            },
            new
            {
                id = "2", name = "Twitter", @ref = "https://twitter.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#1DA1F2" d="M24 4.557a9.93 9.93 0 0 1-2.828.775 4.932 4.932 0 0 0 2.165-2.724 9.864 9.864 0 0 1-3.127 1.195 4.916 4.916 0 0 0-8.38 4.482A13.94 13.94 0 0 1 1.671 3.149a4.916 4.916 0 0 0 1.523 6.573 4.902 4.902 0 0 1-2.228-.616v.062a4.918 4.918 0 0 0 3.946 4.827 4.902 4.902 0 0 1-2.224.085 4.919 4.919 0 0 0 4.588 3.417A9.867 9.867 0 0 1 0 19.54 13.94 13.94 0 0 0 7.548 22c9.057 0 14.01-7.514 14.01-14.01 0-.213-.005-.425-.014-.636A10.025 10.025 0 0 0 24 4.557z"/></svg>"""
            },
            new
            {
                id = "3", name = "Instagram", @ref = "https://instagram.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#E4405F" d="M7 2C4.243 2 2 4.243 2 7v10c0 2.757 2.243 5 5 5h10c2.757 0 5-2.243 5-5V7c0-2.757-2.243-5-5-5H7zm5 5a5 5 0 1 1 0 10 5 5 0 0 1 0-10zm6.5-.75a1.25 1.25 0 1 1-2.5 0 1.25 1.25 0 0 1 2.5 0zM12 9a3 3 0 1 0 0 6 3 3 0 0 0 0-6z"/></svg>"""
            },
            new
            {
                id = "4", name = "YouTube", @ref = "https://youtube.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#FF0000" d="M23.5 6.2a2.9 2.9 0 0 0-2-2C19.6 3.5 12 3.5 12 3.5s-7.6 0-9.5.7a2.9 2.9 0 0 0-2 2A30.4 30.4 0 0 0 0 12a30.4 30.4 0 0 0 .5 5.8 2.9 2.9 0 0 0 2 2c1.9.7 9.5.7 9.5.7s7.6 0 9.5-.7a2.9 2.9 0 0 0 2-2A30.4 30.4 0 0 0 24 12a30.4 30.4 0 0 0-.5-5.8zM9.75 15.02V8.98L15.5 12l-5.75 3.02z"/></svg>"""
            },
            new
            {
                id = "5", name = "LinkedIn", @ref = "https://linkedin.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#0A66C2" d="M20.45 20.45h-3.554v-5.568c0-1.328-.027-3.037-1.851-3.037-1.853 0-2.136 1.445-2.136 2.939v5.666H9.355V9h3.414v1.561h.049c.476-.9 1.637-1.851 3.368-1.851 3.602 0 4.267 2.371 4.267 5.455v6.285zM5.337 7.433a2.063 2.063 0 1 1 0-4.127 2.063 2.063 0 0 1 0 4.127zM6.814 20.45H3.861V9h2.953v11.45z"/></svg>"""
            },
        }));

        // yield return ("config.info", Serialize(new
        // {
        //     id           = "1",
        //     name         = "Ninh. Le Khac",
        //     email        = "ninhlk@nik.com",
        //     phone        = "+84 388 080 661",
        //     address      = "311-313 Đ. Trường Chinh, Khương Thượng, Phương Liệt, Hà Nội 100000, Việt Nam",
        //     website      = "https://nik.com",
        //     avatar       = "https://res.cloudinary.com/worldpackers/image/upload/c_limit,f_auto,q_auto,w_1140/nmfu9poa8vyz3ylmcxax",
        //     bio          = "Writer, photographer, and software developer. I share stories from the road and lessons from the code editor.",
        //     introduction = "Explore stories about fashion, travel, and lifestyle. We share inspiration, trends, and experiences to make everyday life more exciting.",
        // }));
    }

    private static JsonDocument Serialize<T>(T value) =>
        JsonDocument.Parse(JsonSerializer.Serialize(value, _opts));
}
