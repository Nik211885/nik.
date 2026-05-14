using backend.Entities;
using backend.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds demo articles, tags, categories, and threaded comments idempotently on startup.
/// Also creates a demo author user if no users exist.
/// </summary>
public static class ArticleSeeder
{
    /// <summary>
    /// Ensures demo articles with tags, categories, and comments exist in the database.
    /// Skips entirely if any articles are already present.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Articles.AnyAsync()) return;

        var now = DateTimeOffset.UtcNow;

        // ── Author ───────────────────────────────────────────────────────────
        var author = await db.Users.FirstOrDefaultAsync();
        if (author is null)
        {
            var hasher = new PasswordHasher<User>();
            author = new User
            {
                UserName = "Nik",
                Email    = "nik@example.com",
                Password = hasher.HashPassword(null!, "Demo@1234"),
                Bio      = "Writer, photographer, and software developer. I share stories from the road and lessons from the code editor.",
                Avatar   = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=200",
                Slug     = "nik".ToSlug(),
                CreatedDate = now,
                UpdatedDate = now,
            };
            db.Users.Add(author);
        }

        // ── Tags ─────────────────────────────────────────────────────────────
        var tagTech      = MakeTag("technology",   "Technology",   "Software, AI, and emerging tech.",        "https://images.unsplash.com/photo-1518770660439-4636190af475?w=800", now);
        var tagTravel    = MakeTag("travel",        "Travel",       "Destinations and travel stories.",        "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=800", now);
        var tagPhoto     = MakeTag("photography",   "Photography",  "Techniques and visual storytelling.",     "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=800", now);
        var tagLifestyle = MakeTag("lifestyle",     "Lifestyle",    "Habits, productivity, and everyday life.","https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800", now);

        db.Tags.AddRange(tagTech, tagTravel, tagPhoto, tagLifestyle);

        // ── Categories ───────────────────────────────────────────────────────
        var catTech      = MakeCategory("technology",  "Technology",  "Articles about software development, AI, and emerging technologies.", "https://images.unsplash.com/photo-1518770660439-4636190af475?w=800", now);
        var catTravel    = MakeCategory("travel",      "Travel",      "Travel guides, destinations, and road-trip experiences.",             "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=800", now);
        var catPhoto     = MakeCategory("photography", "Photography", "Photography techniques, gear, and visual essays.",                    "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=800", now);
        var catLifestyle = MakeCategory("lifestyle",   "Lifestyle",   "Lifestyle tips, minimalism, and personal growth.",                    "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800", now);

        db.Categories.AddRange(catTech, catTravel, catPhoto, catLifestyle);

        // Save author / tags / categories before articles reference their IDs
        await db.SaveChangesAsync();

        // ── Articles ─────────────────────────────────────────────────────────
        var a0 = MakeArticle("The Future of AI in Everyday Life",
            "Artificial intelligence is quietly reshaping how we work, communicate, and create — here is what to expect in the next five years.",
            "<p>AI is no longer confined to research labs. From smart assistants to personalised medical diagnostics, machine learning models are woven into the fabric of modern life. In this article we explore the key trends driving adoption and what they mean for everyday users.</p><p>Large language models, diffusion models, and reinforcement learning are converging in ways that were unimaginable five years ago. The implications for knowledge work, creative industries, and education are profound.</p>",
            "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=800",
            author.Id, see: 320, like: 45, heart: 28, now.AddDays(-30));

        var a1 = MakeArticle("Exploring Ha Long Bay: A Photographer's Guide",
            "Ha Long Bay offers some of Vietnam's most dramatic coastal scenery — here is everything you need to capture it perfectly.",
            "<p>Rising limestone karsts, emerald waters, and golden-hour light make Ha Long Bay a dream for photographers. The bay changes personality throughout the day, from ethereal morning mist to brilliant afternoon blues and warm sunset tones.</p><p>We cover the best viewpoints, optimal times, and the gear choices that make the difference between a snapshot and a photograph.</p>",
            "https://images.unsplash.com/photo-1528127269322-539801943592?w=800",
            author.Id, see: 270, like: 38, heart: 22, now.AddDays(-25));

        var a2 = MakeArticle("Mastering Street Photography",
            "Street photography is one of the most rewarding and challenging genres — these techniques will help you capture authentic moments.",
            "<p>The decisive moment, as Cartier-Bresson called it, is both a technical and intuitive discipline. Pre-focusing at a fixed distance, working with available light, and learning to become invisible to your subjects are skills that take time but transform results.</p><p>This guide covers everything from lens selection to the ethics of photographing strangers in public spaces.</p>",
            "https://images.unsplash.com/photo-1477959858617-67f85cf4f1df?w=800",
            author.Id, see: 210, like: 30, heart: 18, now.AddDays(-20));

        var a3 = MakeArticle("Building a Morning Routine That Actually Works",
            "Most morning routines fail because they ignore personal chronobiology — here is how to design one that sticks.",
            "<p>The key to a sustainable morning routine is understanding your natural energy rhythms rather than copying someone else's 5 AM schedule. Chronobiology research shows that optimal wake times vary significantly between individuals.</p><p>This article walks through evidence-based strategies for building a routine that enhances rather than drains your energy throughout the day.</p>",
            "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800",
            author.Id, see: 180, like: 25, heart: 15, now.AddDays(-18));

        var a4 = MakeArticle("TypeScript 5.0: What's New and Why It Matters",
            "TypeScript 5.0 brings decorators, const type parameters, and significant performance improvements — here is a practical walkthrough.",
            "<p>TypeScript 5.0 is the most significant release since version 4.0. The new decorator standard finally aligns with the TC39 proposal, improved enum handling eliminates a class of long-standing bugs, and bundle-size reductions improve cold-start performance across large projects.</p><p>We walk through each change with practical code examples and migration tips.</p>",
            "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800",
            author.Id, see: 290, like: 42, heart: 26, now.AddDays(-15));

        var a5 = MakeArticle("Japan in Autumn: A Week in Kyoto",
            "Kyoto's autumn colours are legendary — this is a day-by-day itinerary for experiencing the best of the season.",
            "<p>From the vermilion torii gates of Fushimi Inari reflected in fallen leaves to the golden pavilion of Kinkakuji framed by maples, Kyoto's autumn season is unlike anywhere else on earth. The koyo season typically peaks in mid-November, but crowds arrive weeks earlier.</p><p>This day-by-day itinerary balances iconic landmarks with the quiet temple gardens most visitors miss.</p>",
            "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=800",
            author.Id, see: 340, like: 50, heart: 32, now.AddDays(-12));

        var a6 = MakeArticle("Understanding Docker for Developers",
            "Containers have changed how we build and ship software — this guide demystifies Docker for developers coming from a traditional setup.",
            "<p>Docker solves the 'works on my machine' problem by packaging applications with their entire runtime environment into portable containers. Understanding images, layers, volumes, and networking unlocks a fundamentally better way to develop and deploy software.</p><p>This tutorial is aimed at developers who have heard about Docker but have not yet made it part of their daily workflow.</p>",
            "https://images.unsplash.com/photo-1605745341112-85968b19335b?w=800",
            author.Id, see: 260, like: 36, heart: 21, now.AddDays(-10));

        var a7 = MakeArticle("The Art of Landscape Photography",
            "Great landscape photography goes beyond pointing at beautiful scenery — composition, light, and patience are everything.",
            "<p>Understanding the relationship between foreground interest, leading lines, and the sky is fundamental to compelling landscape photography. The golden hours — the first and last hour of daylight — offer soft directional light that transforms ordinary scenes into extraordinary ones.</p><p>We cover techniques for every experience level, from the basics of the rule of thirds to advanced ND filter stacking.</p>",
            "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800",
            author.Id, see: 195, like: 28, heart: 17, now.AddDays(-8));

        var a8 = MakeArticle("Minimalism in Digital Life",
            "Digital clutter costs you more than you think — here is how to apply minimalism to your devices, apps, and online habits.",
            "<p>Between notifications, subscription services, and endless algorithmic feeds, our digital environments have become overwhelming. The cognitive load of managing a cluttered digital life quietly drains attention and creative energy.</p><p>This guide offers practical steps for simplifying your digital world — from your home screen to your email inbox to your social media habits.</p>",
            "https://images.unsplash.com/photo-1484480974693-6ca0a78fb36b?w=800",
            author.Id, see: 155, like: 22, heart: 13, now.AddDays(-6));

        var a9 = MakeArticle("Vietnam by Motorcycle: The Ho Chi Minh Trail",
            "Riding the Ho Chi Minh trail from north to south is one of Southeast Asia's great adventures — here is how to plan yours.",
            "<p>The Ho Chi Minh trail cuts through remote mountain passes, sleepy highland villages, and some of the most dramatic scenery in Southeast Asia. Riding it on a semi-automatic motorbike is a rite of passage for travellers in Vietnam.</p><p>We cover route planning from Hanoi to Ho Chi Minh City, essential gear, the best homestays, and the hidden stops most tour groups miss.</p>",
            "https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=800",
            author.Id, see: 230, like: 33, heart: 20, now.AddDays(-4));

        var a10 = MakeArticle("React vs Angular in 2025",
            "Two of the most popular frontend frameworks continue to evolve — here is an honest comparison for teams choosing a stack in 2025.",
            "<p>React's flexibility and Angular's opinionated structure serve genuinely different team needs. React's ecosystem breadth and Angular's built-in tooling represent different philosophies about where decisions should live.</p><p>This article compares developer experience, performance characteristics, ecosystem maturity, and long-term maintainability to help you make an informed decision for your next project.</p>",
            "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800",
            author.Id, see: 300, like: 44, heart: 27, now.AddDays(-2));

        var a11 = MakeArticle("Light and Shadow: Portrait Photography Basics",
            "Understanding how light falls on the human face is the single most important skill in portrait photography.",
            "<p>Whether you are shooting with natural window light or a studio strobe, the classical lighting patterns — Rembrandt, loop, butterfly, and split — apply equally. Each creates a different mood and flatters different face shapes.</p><p>This guide breaks down each lighting pattern with practical setup diagrams and real-world examples, so you can apply them with whatever light source you have available.</p>",
            "https://images.unsplash.com/photo-1531746020798-e6953c6e8e04?w=800",
            author.Id, see: 175, like: 26, heart: 16, now.AddDays(-1));

        db.Articles.AddRange(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);

        // ── Tag / Category junctions ──────────────────────────────────────────
        //  a0  AI              → tech + lifestyle  /  catTech
        //  a1  Ha Long Bay     → travel + photo    /  catTravel
        //  a2  Street Photo    → photo + lifestyle /  catPhoto
        //  a3  Morning Routine → lifestyle         /  catLifestyle
        //  a4  TypeScript      → tech              /  catTech
        //  a5  Japan Autumn    → travel + photo    /  catTravel
        //  a6  Docker          → tech              /  catTech
        //  a7  Landscape Photo → photo             /  catPhoto
        //  a8  Minimalism      → lifestyle         /  catLifestyle
        //  a9  Vietnam Moto    → travel            /  catTravel
        //  a10 React vs Angular→ tech              /  catTech
        //  a11 Portrait Photo  → photo             /  catPhoto
        AddJunctions(db, a0,  tags: [tagTech, tagLifestyle], cats: [catTech]);
        AddJunctions(db, a1,  tags: [tagTravel, tagPhoto],   cats: [catTravel]);
        AddJunctions(db, a2,  tags: [tagPhoto, tagLifestyle],cats: [catPhoto]);
        AddJunctions(db, a3,  tags: [tagLifestyle],          cats: [catLifestyle]);
        AddJunctions(db, a4,  tags: [tagTech],               cats: [catTech]);
        AddJunctions(db, a5,  tags: [tagTravel, tagPhoto],   cats: [catTravel]);
        AddJunctions(db, a6,  tags: [tagTech],               cats: [catTech]);
        AddJunctions(db, a7,  tags: [tagPhoto],              cats: [catPhoto]);
        AddJunctions(db, a8,  tags: [tagLifestyle],          cats: [catLifestyle]);
        AddJunctions(db, a9,  tags: [tagTravel],             cats: [catTravel]);
        AddJunctions(db, a10, tags: [tagTech],               cats: [catTech]);
        AddJunctions(db, a11, tags: [tagPhoto],              cats: [catPhoto]);

        // tagTech:      a0 a4 a6 a10     = 4
        // tagTravel:    a1 a5 a9         = 3
        // tagPhoto:     a1 a2 a5 a7 a11  = 5
        // tagLifestyle: a0 a2 a3 a8      = 4
        tagTech.CountRef      = 4;
        tagTravel.CountRef    = 3;
        tagPhoto.CountRef     = 5;
        tagLifestyle.CountRef = 4;

        // catTech:      a0 a4 a6 a10     = 4
        // catTravel:    a1 a5 a9         = 3
        // catPhoto:     a2 a7 a11        = 3
        // catLifestyle: a3 a8            = 2
        catTech.CountRef      = 4;
        catTravel.CountRef    = 3;
        catPhoto.CountRef     = 3;
        catLifestyle.CountRef = 2;

        // ── Comments ──────────────────────────────────────────────────────────
        // Each article gets 3 top-level comments and 2 replies (total 5 per article).
        foreach (var article in new[] { a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11 })
        {
            var c1 = MakeComment(article.Id, author.Id,
                "Really insightful read. I've been following this topic closely and this article nailed all the key points.",
                parentId: null, now.AddHours(-20));

            var c2 = MakeComment(article.Id, author.Id,
                "Great breakdown. Would love to see a follow-up focusing on practical implementation.",
                parentId: null, now.AddHours(-16));

            var c3 = MakeComment(article.Id, author.Id,
                "This is exactly what I was looking for. Bookmarked and sharing with my team straight away.",
                parentId: null, now.AddHours(-10));

            var r1 = MakeComment(article.Id, author.Id,
                "Glad it helped! A follow-up post is already in the works for next month.",
                parentId: c1.Id, now.AddHours(-8));

            var r2 = MakeComment(article.Id, author.Id,
                "Thanks for the kind words — definitely more content coming on this soon.",
                parentId: c2.Id, now.AddHours(-4));

            db.Comments.AddRange(c1, c2, c3, r1, r2);
            article.CountCommentRef = 5; // 3 top-level + 2 replies
        }

        await db.SaveChangesAsync();
    }

    // ── Factory helpers ───────────────────────────────────────────────────────

    private static Article MakeArticle(
        string title, string description, string content,
        string image, string authorId,
        int see, int like, int heart,
        DateTimeOffset createdDate) => new()
    {
        Title       = title,
        Description = description,
        Content     = content,
        Image       = image,
        Slug        = title.ToSlug(),
        AuthorId    = authorId,
        CountSee      = see,
        CountLikeRef  = like,
        CountHeartRef = heart,
        CreatedDate = createdDate,
        UpdatedDate = createdDate,
    };

    private static Tag MakeTag(string name, string title, string description, string image, DateTimeOffset now) => new()
    {
        Name        = name,
        Title       = title,
        Description = description,
        Image       = image,
        Slug        = name.ToSlug(),
        CreatedDate = now,
        UpdatedDate = now,
    };

    private static Category MakeCategory(string name, string title, string description, string image, DateTimeOffset now) => new()
    {
        Name        = name,
        Title       = title,
        Description = description,
        Image       = image,
        Slug        = name.ToSlug(),
        CreatedDate = now,
        UpdatedDate = now,
    };

    private static Comment MakeComment(string articleId, string authorId, string text, string? parentId, DateTimeOffset createdDate) => new()
    {
        ArticleId   = articleId,
        AuthorId    = authorId,
        Text        = text,
        ParentId    = parentId,
        CreatedDate = createdDate,
    };

    private static void AddJunctions(ApplicationDbContext db, Article article, Tag[] tags, Category[] cats)
    {
        foreach (var tag in tags)
            db.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tag.Id });
        foreach (var cat in cats)
            db.ArticleCategories.Add(new ArticleCategory { ArticleId = article.Id, CategoryId = cat.Id });
    }
}
