using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds English (en) ContentTranslation records for all Vietnamese entities.
/// Must run after ArticleSeeder, AlbumSeeder, and HeroSlideSeeder.
/// </summary>
public static class ContentTranslationSeeder
{
    /// <summary>Seeds English translations. Skips if any English translation already exists.</summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.ContentTranslations.AnyAsync(t => t.LangCode == "en")) return;

        var translations = new List<ContentTranslation>();

        await SeedTagsAsync(db, translations);
        await SeedCategoriesAsync(db, translations);
        await SeedAlbumsAsync(db, translations);
        await SeedHeroSlidesAsync(db, translations);
        await SeedArticlesAsync(db, translations);

        db.ContentTranslations.AddRange(translations);
        await db.SaveChangesAsync();
    }

    // ── Tags ──────────────────────────────────────────────────────────────────

    private static async Task SeedTagsAsync(ApplicationDbContext db, List<ContentTranslation> result)
    {
        var tags = await db.Tags.AsNoTracking().ToListAsync();

        // Vi name → (en title, en description)
        var map = new Dictionary<string, (string Title, string Desc)>
        {
            ["lap-trinh"]        = ("Programming",           "Articles about programming, code, and software technology."),
            ["tri-tue-nhan-tao"] = ("Artificial Intelligence","AI, machine learning, and emerging technology trends."),
            ["du-lich"]          = ("Travel",                 "Stories and experiences from journeys around the world."),
            ["nhiep-anh"]        = ("Photography",            "Techniques and the art of photography."),
            ["phong-cach-song"]  = ("Lifestyle",              "Habits, productivity, and everyday life."),
            ["thoi-trang"]       = ("Fashion",                "Contemporary fashion trends and personal style."),
        };

        foreach (var tag in tags)
        {
            if (!map.TryGetValue(tag.Name, out var t)) continue;
            result.Add(CT(EntityType.Tag, tag.Id, "title",       t.Title));
            result.Add(CT(EntityType.Tag, tag.Id, "description", t.Desc));
        }
    }

    // ── Categories ────────────────────────────────────────────────────────────

    private static async Task SeedCategoriesAsync(ApplicationDbContext db, List<ContentTranslation> result)
    {
        var cats = await db.Categories.AsNoTracking().ToListAsync();

        var map = new Dictionary<string, (string Title, string Desc)>
        {
            ["cong-nghe"] = ("Technology", "Articles about programming, AI, and emerging technologies."),
            ["du-lich"]   = ("Travel",     "Travel guides, destinations, and on-the-road experiences."),
            ["nhiep-anh"] = ("Photography","Techniques, gear, and the art of photography."),
            ["doi-song"]  = ("Lifestyle",  "Lifestyle, minimalism, and personal development."),
        };

        foreach (var cat in cats)
        {
            if (!map.TryGetValue(cat.Name, out var t)) continue;
            result.Add(CT(EntityType.Category, cat.Id, "title", t.Title));
        }
    }

    // ── Albums ────────────────────────────────────────────────────────────────

    private static async Task SeedAlbumsAsync(ApplicationDbContext db, List<ContentTranslation> result)
    {
        var albums = await db.Albums.AsNoTracking().ToListAsync();

        var map = new Dictionary<string, (string Title, string Desc)>
        {
            ["nhiep-anh"]   = ("Photography",  "A collection of photographic work."),
            ["blog"]        = ("Blog",         "Images and assets used in blog posts."),
            ["du-an"]       = ("Projects",     "Screenshots and assets for projects."),
            ["du-lich"]     = ("Travel",       "Photos captured while travelling."),
            ["thien-nhien"] = ("Nature",       "Landscapes, flora, and wildlife."),
            ["thoi-trang"]  = ("Fashion",      "Fashion and portrait photography."),
            ["duong-pho"]   = ("Street",       "Urban street photography."),
            ["nhat-ban"]    = ("Japan",        "Land of the rising sun."),
            ["viet-nam"]    = ("Vietnam",      "The beauty of the S-shaped land."),
            ["chau-au"]     = ("Europe",       "European adventures."),
            ["rung"]        = ("Forest",       "Deep inside the forest."),
            ["bien"]        = ("Ocean",        "Beneath and beside the sea."),
            ["anh-bia"]     = ("Covers",       "Article cover images."),
            ["banner"]      = ("Banners",      "Homepage hero banners."),
        };

        foreach (var album in albums)
        {
            if (!map.TryGetValue(album.Name, out var t)) continue;
            result.Add(CT(EntityType.Album, album.Id, "title",       t.Title));
            result.Add(CT(EntityType.Album, album.Id, "description", t.Desc));
        }
    }

    // ── Hero Slides ───────────────────────────────────────────────────────────

    private static async Task SeedHeroSlidesAsync(ApplicationDbContext db, List<ContentTranslation> result)
    {
        var slides = await db.HeroSlides.AsNoTracking().OrderBy(s => s.OrderIndex).ToListAsync();

        var map = new Dictionary<string, (string Title, string Desc)>
        {
            ["Khám phá Thế giới"]      = ("Explore the World",         "Where stories about travel, photography, and lifestyle come together."),
            ["Hành trình và Cảm hứng"] = ("Journey and Inspiration",   "Sharing perspectives and experiences from travels around the world."),
        };

        foreach (var slide in slides)
        {
            if (!map.TryGetValue(slide.Title, out var t)) continue;
            result.Add(CT(EntityType.HeroSlide, slide.Id, "title",       t.Title));
            result.Add(CT(EntityType.HeroSlide, slide.Id, "description", t.Desc));
        }
    }

    // ── Articles ──────────────────────────────────────────────────────────────

    private static async Task SeedArticlesAsync(ApplicationDbContext db, List<ContentTranslation> result)
    {
        var articles = await db.Articles.AsNoTracking().OrderBy(a => a.CreatedDate).ToListAsync();

        // Keyed by Vietnamese title → (en title, en description, en content)
        var map = new Dictionary<string, (string Title, string Desc, string Content)>
        {
            ["Tương lai của Trí tuệ nhân tạo trong cuộc sống hàng ngày"] = (
                "The Future of Artificial Intelligence in Everyday Life",
                "Artificial intelligence is quietly reshaping how we work, communicate, and create — here is what to expect in the next five years.",
                "<p>AI is no longer confined to research labs. From smart assistants to personalised medical diagnostics, machine learning models are woven into the fabric of modern life. We explore the key trends driving AI adoption and what they mean for everyday users.</p><p>Large language models, diffusion models, and reinforcement learning are converging in ways that were unimaginable just a few years ago. The implications for knowledge work, creative industries, and education are profound.</p>"),

            ["Khám phá Vịnh Hạ Long: Góc nhìn của một nhiếp ảnh gia"] = (
                "Exploring Ha Long Bay: A Photographer's Perspective",
                "Ha Long Bay offers some of Vietnam's most dramatic coastal scenery — here is everything you need to capture the perfect shot.",
                "<p>Rising limestone karsts, emerald waters, and golden-hour light make Ha Long Bay a paradise for photographers. The bay changes character throughout the day, from ethereal morning mist to brilliant afternoon blues and warm sunset tones.</p><p>We share the best viewpoints, ideal timing, and gear choices that make the difference between a snapshot and a truly memorable photograph.</p>"),

            ["Nhiếp ảnh đường phố: Bắt trọn khoảnh khắc thật"] = (
                "Street Photography: Capturing Authentic Moments",
                "Street photography is one of the most rewarding and challenging genres — these techniques will help you capture real, candid moments.",
                "<p>The decisive moment, as Cartier-Bresson called it, is both a technical and intuitive discipline. Pre-focusing at a fixed distance, working with available light, and learning to become invisible to your subjects are skills that transform results.</p><p>This guide covers everything from lens selection to the ethics of photographing strangers in public spaces, helping you take to the streets with confidence.</p>"),

            ["Xây dựng thói quen buổi sáng thực sự hiệu quả"] = (
                "Building a Morning Routine That Actually Works",
                "Most morning routines fail because they ignore personal chronobiology — here is how to design one that truly sticks.",
                "<p>The key to a sustainable morning routine is understanding your natural energy rhythms rather than copying someone else's 5 AM schedule. Chronobiology research shows that optimal wake times vary significantly between individuals.</p><p>This article walks through evidence-based strategies for building a routine that enhances rather than drains your energy throughout the working day.</p>"),

            ["TypeScript 5.0: Những tính năng mới đáng chú ý"] = (
                "TypeScript 5.0: New Features Worth Knowing",
                "TypeScript 5.0 brings decorators, const type parameters, and significant performance improvements — here is a practical walkthrough.",
                "<p>TypeScript 5.0 is the most significant release since version 4.0. The new decorator standard finally aligns with the TC39 proposal, improved enum handling eliminates a class of long-standing bugs, and bundle-size reductions improve cold-start performance on large projects.</p><p>We walk through each change with practical code examples and migration tips to help you upgrade smoothly and take full advantage of the new features.</p>"),

            ["Nhật Bản mùa thu: Một tuần trải nghiệm tại Kyoto"] = (
                "Japan in Autumn: A Week Experiencing Kyoto",
                "Kyoto's autumn colours are legendary — here is a day-by-day itinerary for the most complete experience of the red-leaf season.",
                "<p>From the vermilion torii gates of Fushimi Inari reflected in fallen leaves to the golden pavilion of Kinkakuji framed by blazing maples, Kyoto's autumn season is unlike anywhere else on earth. The koyo season typically peaks in mid-November.</p><p>This day-by-day itinerary balances iconic landmarks with the quiet temple gardens that most visitors overlook on their journey.</p>"),

            ["Docker cho lập trình viên: Từ cơ bản đến thực chiến"] = (
                "Docker for Developers: From Basics to Real-World Use",
                "Containers have changed how we build and ship software — this guide explains Docker clearly for developers starting from scratch.",
                "<p>Docker solves the 'works on my machine' problem by packaging applications with their entire runtime environment into portable containers. Understanding images, layers, volumes, and networking unlocks a fundamentally better way to develop and deploy software.</p><p>This guide is aimed at developers who have heard about Docker but have not yet made it part of their daily workflow, with practical examples from simple to complex.</p>"),

            ["Nghệ thuật nhiếp ảnh phong cảnh"] = (
                "The Art of Landscape Photography",
                "Great landscape photography goes beyond pointing a camera at beautiful scenery — composition, light, and patience are the deciding factors.",
                "<p>Understanding the relationship between foreground interest, leading lines, and the sky is fundamental to compelling landscape photography. The golden hours — the first and last hour of daylight — offer soft directional light that transforms ordinary scenes into extraordinary ones.</p><p>We cover techniques for every level, from the basics of the rule of thirds to advanced ND filter stacking, to help you capture more impressive landscape shots.</p>"),

            ["Sống tối giản trong thế giới số"] = (
                "Living Minimally in the Digital World",
                "Digital clutter costs you more than you think — here is how to apply minimalism to your devices, apps, and online habits.",
                "<p>Between notifications, subscription services, and endless algorithmic feeds, our digital environments have become overwhelming. The cognitive load of managing a cluttered digital life quietly drains attention and creative energy.</p><p>This guide offers practical steps for simplifying your digital world — from your home screen to your email inbox to your daily social media habits.</p>"),

            ["Hành trình xuyên Việt trên chiếc xe máy"] = (
                "Crossing Vietnam by Motorbike",
                "Riding from north to south along the Ho Chi Minh Road is one of Southeast Asia's great adventures — here is how to plan yours.",
                "<p>The Ho Chi Minh Road cuts through remote mountain passes, sleepy highland villages, and some of the most dramatic scenery in Southeast Asia. Riding a manual motorbike from Hanoi to Ho Chi Minh City is a rite of passage for Vietnam travel enthusiasts.</p><p>We share a detailed route plan, essential gear, the best places to stay, and the hidden stops that most tour groups miss along the way.</p>"),

            ["React và Angular năm 2025: So sánh toàn diện"] = (
                "React vs Angular in 2025: A Comprehensive Comparison",
                "The two most popular frontend frameworks continue to evolve — here is an honest comparison for teams choosing their tech stack in 2025.",
                "<p>React's flexibility and Angular's opinionated structure serve genuinely different team needs. React's ecosystem breadth and Angular's built-in tooling represent different philosophies about where architectural decisions should live.</p><p>This article compares developer experience, performance characteristics, ecosystem maturity, and long-term maintainability to help you make an informed decision for your next project.</p>"),

            ["Ánh sáng và bóng tối: Căn bản nhiếp ảnh chân dung"] = (
                "Light and Shadow: Portrait Photography Fundamentals",
                "Understanding how light falls on the human face is the single most important skill in portrait photography, whatever your level.",
                "<p>Whether you are shooting with natural window light or a studio strobe, the classical lighting patterns — Rembrandt, loop, butterfly, and split — apply equally. Each creates a different mood and flatters different face shapes.</p><p>This guide breaks down each lighting pattern with practical setup diagrams and real-world examples so you can apply them with whatever light source is available.</p>"),
        };

        foreach (var article in articles)
        {
            if (!map.TryGetValue(article.Title, out var t)) continue;
            result.Add(CT(EntityType.Article, article.Id, "title",       t.Title));
            result.Add(CT(EntityType.Article, article.Id, "description", t.Desc));
            result.Add(CT(EntityType.Article, article.Id, "content",     t.Content));
        }
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static ContentTranslation CT(string entityType, string entityId, string field, string value) => new()
    {
        EntityType = entityType,
        EntityId   = entityId,
        Field      = field,
        LangCode   = "en",
        Value      = value,
    };
}
