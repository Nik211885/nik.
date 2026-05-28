using System.Text.Json;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds system configuration entries using UPSERT logic.
/// Text fields that differ by language use <c>{"vi":"...","en":"..."}</c> JSON — the backend
/// automatically extracts the value matching the request language at read time.
/// </summary>
public static class SysConfigSeeder
{
    private static readonly JsonSerializerOptions _opts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>Seeds config entries. Inserts missing keys; updates value of existing keys.</summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var existing = await db.SysConfigs.ToDictionaryAsync(x => x.Key);

        foreach (var (key, value) in BuildEntries())
        {
            if (existing.TryGetValue(key, out var row))
                row.Value = value;
            else
                db.SysConfigs.Add(new SysConfig { Key = key, Value = value });
        }

        await db.SaveChangesAsync();
    }

    private static IEnumerable<(string Key, JsonDocument Value)> BuildEntries()
    {
        // ── Sidebar navigation ─────────────────────────────────────────────
        yield return ("config.sidebar", Serialize(new[]
        {
            new { nameKey = "nav.home",        @ref = "/" },
            new { nameKey = "nav.photography", @ref = "/photography" },
            new { nameKey = "nav.travel",      @ref = "/travel" },
            new { nameKey = "nav.fashion",     @ref = "/fashion" },
            new { nameKey = "nav.about",       @ref = "/about" },
            new { nameKey = "nav.contact",     @ref = "/contact" },
            new { nameKey = "nav.wall",          @ref = "/wall" },
            new { nameKey = "nav.sponsor",       @ref = "/sponsor" },
            new { nameKey = "nav.vietnam-map",   @ref = "/vietnam-map" },
        }));

        // ── Social links ───────────────────────────────────────────────────
        yield return ("config.social", Serialize(new[]
        {
            new
            {
                id = "1", name = "Facebook", @ref = "https://facebook.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#1877F2" d="M24 12a12 12 0 1 0-13.875 11.85v-8.385H7.078V12h3.047V9.356c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.926-1.956 1.874V12h3.328l-.532 3.465h-2.796v8.385A12 12 0 0 0 24 12z"/></svg>"""
            },
            new
            {
                id = "2", name = "Instagram", @ref = "https://instagram.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#E4405F" d="M7 2C4.243 2 2 4.243 2 7v10c0 2.757 2.243 5 5 5h10c2.757 0 5-2.243 5-5V7c0-2.757-2.243-5-5-5H7zm5 5a5 5 0 1 1 0 10 5 5 0 0 1 0-10zm6.5-.75a1.25 1.25 0 1 1-2.5 0 1.25 1.25 0 0 1 2.5 0zM12 9a3 3 0 1 0 0 6 3 3 0 0 0 0-6z"/></svg>"""
            },
            new
            {
                id = "3", name = "YouTube", @ref = "https://youtube.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#FF0000" d="M23.5 6.2a2.9 2.9 0 0 0-2-2C19.6 3.5 12 3.5 12 3.5s-7.6 0-9.5.7a2.9 2.9 0 0 0-2 2A30.4 30.4 0 0 0 0 12a30.4 30.4 0 0 0 .5 5.8 2.9 2.9 0 0 0 2 2c1.9.7 9.5.7 9.5.7s7.6 0 9.5-.7a2.9 2.9 0 0 0 2-2A30.4 30.4 0 0 0 24 12a30.4 30.4 0 0 0-.5-5.8zM9.75 15.02V8.98L15.5 12l-5.75 3.02z"/></svg>"""
            },
            new
            {
                id = "4", name = "GitHub", @ref = "https://github.com",
                icon = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="#181717" d="M12 .5C5.65.5.5 5.65.5 12c0 5.1 3.3 9.4 7.9 10.9.58.1.79-.25.79-.55v-1.93c-3.2.7-3.88-1.54-3.88-1.54-.52-1.33-1.28-1.68-1.28-1.68-1.05-.72.08-.7.08-.7 1.16.08 1.77 1.19 1.77 1.19 1.03 1.76 2.7 1.25 3.36.96.1-.75.4-1.25.73-1.54-2.56-.29-5.25-1.28-5.25-5.7 0-1.26.45-2.29 1.18-3.1-.12-.3-.51-1.47.11-3.06 0 0 .97-.31 3.17 1.18a11.05 11.05 0 0 1 5.78 0c2.2-1.49 3.16-1.18 3.16-1.18.63 1.59.24 2.76.12 3.06.74.81 1.18 1.84 1.18 3.1 0 4.43-2.7 5.41-5.27 5.69.41.36.78 1.06.78 2.13v3.16c0 .31.21.66.8.55A11.5 11.5 0 0 0 23.5 12C23.5 5.65 18.35.5 12 .5z"/></svg>"""
            },
        }));

        // ── Thông tin cá nhân — multilingual với {"vi":...,"en":...} ────────
        yield return ("config.info", Serialize(new
        {
            vi = new
            {
                name         = "Lê Khắc Ninh",
                email        = "ninhlk@nik.com",
                phone        = "+84 388 080 661",
                address      = "311-313 Đ. Trường Chinh, Khương Thượng, Phương Liệt, Hà Nội 100000, Việt Nam",
                website      = "https://nik.com",
                avatar       = "https://res.cloudinary.com/djvpvcj9g/image/upload/v1778947411/593829710_707055712480882_4581758384697286757_n_jyqgwu.jpg",
                bio          = "Lập trình viên, nhiếp ảnh gia và người yêu thích du lịch. Tôi chia sẻ những câu chuyện từ những chuyến đi và bài học từ công việc lập trình hằng ngày.",
                introduction = "Chào mừng đến với không gian chia sẻ của tôi — nơi hội tụ những câu chuyện về du lịch, nhiếp ảnh, thời trang và công nghệ. Mỗi bài viết là một góc nhìn, một trải nghiệm, một khoảnh khắc tôi muốn lưu giữ và chia sẻ cùng bạn.",
            },
            en = new
            {
                name         = "Ninh Le Khac",
                email        = "ninhlk@nik.com",
                phone        = "+84 388 080 661",
                address      = "311-313 Truong Chinh, Khuong Thuong, Phuong Liet, Hanoi 100000, Vietnam",
                website      = "https://nik.com",
                avatar       = "https://res.cloudinary.com/djvpvcj9g/image/upload/v1778947411/593829710_707055712480882_4581758384697286757_n_jyqgwu.jpg",
                bio          = "Developer, photographer, and travel enthusiast. I share stories from the road and lessons from the code editor.",
                introduction = "Welcome to my personal space — where stories about travel, photography, fashion, and technology come together. Each post is a perspective, an experience, a moment I want to preserve and share with you.",
            },
        }));
    }

    private static JsonDocument Serialize<T>(T value) =>
        JsonDocument.Parse(JsonSerializer.Serialize(value, _opts));
}
