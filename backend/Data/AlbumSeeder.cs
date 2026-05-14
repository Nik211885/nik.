using backend.Entities;
using backend.Extensions;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds sample album hierarchy and demo files idempotently on startup.
/// </summary>
public static class AlbumSeeder
{
    /// <summary>
    /// Ensures a sample album tree and demo files exist in the database.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Albums.AnyAsync()) return;

        var now = DateTimeOffset.UtcNow;

        // ── Root albums ──────────────────────────────────────────────────────
        var photography = MakeAlbum("photography", "Photography", "A collection of photographic work.", null, 1, now);
        var blog        = MakeAlbum("blog",        "Blog",        "Visual assets used in blog posts.",  null, 2, now);
        var projects    = MakeAlbum("projects",    "Projects",    "Screenshots and assets for projects.", null, 3, now);

        // ── Photography children ─────────────────────────────────────────────
        var travel  = MakeAlbum("travel",   "Travel",   "Photos captured while travelling.",  photography.Id, 1, now);
        var nature  = MakeAlbum("nature",   "Nature",   "Landscapes, flora and fauna.",        photography.Id, 2, now);
        var fashion = MakeAlbum("fashion",  "Fashion",  "Fashion and portrait photography.",   photography.Id, 3, now);
        var street  = MakeAlbum("street",   "Street",   "Urban street photography.",           photography.Id, 4, now);

        // ── Travel children ──────────────────────────────────────────────────
        var japan   = MakeAlbum("japan",    "Japan",    "Land of the rising sun.",   travel.Id, 1, now);
        var vietnam = MakeAlbum("vietnam",  "Vietnam",  "Beautiful Vietnam.",        travel.Id, 2, now);
        var europe  = MakeAlbum("europe",   "Europe",   "European adventures.",      travel.Id, 3, now);

        // ── Nature children ──────────────────────────────────────────────────
        var forest  = MakeAlbum("forest",   "Forest",    "Deep inside the forest.",  nature.Id, 1, now);
        var ocean   = MakeAlbum("ocean",    "Ocean",     "Beneath and beside the sea.", nature.Id, 2, now);

        // ── Blog children ────────────────────────────────────────────────────
        var covers  = MakeAlbum("covers",   "Covers",   "Article cover images.",     blog.Id, 1, now);
        var banners = MakeAlbum("banners",  "Banners",  "Hero banners.",             blog.Id, 2, now);

        db.Albums.AddRange(
            photography, blog, projects,
            travel, nature, fashion, street,
            japan, vietnam, europe,
            forest, ocean,
            covers, banners
        );

        // ── Demo files for leaf albums ────────────────────────────────────────
        var demoImages = new[]
        {
            // Japan
            MakeFile("tokyo-tower",       "Tokyo Tower",        "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800"),
            MakeFile("shibuya-crossing",  "Shibuya Crossing",   "https://images.unsplash.com/photo-1542051841857-5f90071e7989?w=800"),
            MakeFile("mount-fuji",        "Mount Fuji",         "https://images.unsplash.com/photo-1578637387939-43c525550085?w=800"),

            // Vietnam
            MakeFile("ha-long-bay",       "Ha Long Bay",        "https://images.unsplash.com/photo-1528127269322-539801943592?w=800"),
            MakeFile("hoi-an-lanterns",   "Hoi An Lanterns",    "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800"),

            // Europe
            MakeFile("paris-eiffel",      "Eiffel Tower",       "https://images.unsplash.com/photo-1511739001486-6bfe10ce785f?w=800"),
            MakeFile("santorini",         "Santorini",          "https://images.unsplash.com/photo-1469796466635-455ede028aca?w=800"),

            // Forest
            MakeFile("misty-forest",      "Misty Forest",       "https://images.unsplash.com/photo-1448375240586-882707db888b?w=800"),
            MakeFile("sunlit-trees",      "Sunlit Trees",       "https://images.unsplash.com/photo-1426604966848-d7adac402bff?w=800"),

            // Ocean
            MakeFile("ocean-sunset",      "Ocean Sunset",       "https://images.unsplash.com/photo-1505118380757-91f5f5632de0?w=800"),
            MakeFile("coral-reef",        "Coral Reef",         "https://images.unsplash.com/photo-1518020382113-a7e8fc38eac9?w=800"),

            // Fashion
            MakeFile("fashion-portrait",  "Fashion Portrait",   "https://images.unsplash.com/photo-1529626455594-4ff0802cfb7e?w=800"),
            MakeFile("street-style",      "Street Style",       "https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=800"),

            // Street
            MakeFile("city-lights",       "City Lights",        "https://images.unsplash.com/photo-1477959858617-67f85cf4f1df?w=800"),
            MakeFile("urban-geometry",    "Urban Geometry",     "https://images.unsplash.com/photo-1486325212027-8081e485255e?w=800"),

            // Covers
            MakeFile("blog-cover-1",      "Cover 1",            "https://images.unsplash.com/photo-1499750310107-5fef28a66643?w=800"),
            MakeFile("blog-cover-2",      "Cover 2",            "https://images.unsplash.com/photo-1432821596592-e2c18b78144f?w=800"),

            // Banners
            MakeFile("hero-banner-1",     "Hero Banner 1",      "https://images.unsplash.com/photo-1493612276216-ee3925520721?w=800"),
            MakeFile("hero-banner-2",     "Hero Banner 2",      "https://images.unsplash.com/photo-1518655048521-f130df041f66?w=800"),
        };

        db.files.AddRange(demoImages);

        // ── AlbumFile junction records ────────────────────────────────────────
        var map = new Dictionary<Album, backend.Entities.File[]>
        {
            [japan]   = [demoImages[0],  demoImages[1],  demoImages[2]],
            [vietnam] = [demoImages[3],  demoImages[4]],
            [europe]  = [demoImages[5],  demoImages[6]],
            [forest]  = [demoImages[7],  demoImages[8]],
            [ocean]   = [demoImages[9],  demoImages[10]],
            [fashion] = [demoImages[11], demoImages[12]],
            [street]  = [demoImages[13], demoImages[14]],
            [covers]  = [demoImages[15], demoImages[16]],
            [banners] = [demoImages[17], demoImages[18]],
        };

        foreach (var (album, files) in map)
        {
            foreach (var file in files)
                db.AlbumFiles.Add(new AlbumFile { AlbumId = album.Id, FileId = file.Id });
            album.CountImageRef = files.Length;
        }

        await db.SaveChangesAsync();
    }

    private static Album MakeAlbum(string name, string title, string description, string? parentId, int order, DateTimeOffset now) =>
        new()
        {
            Name = name,
            Title = title,
            Description = description,
            Slug = name.ToSlug(),
            AlbumId = parentId,
            OrderIndex = order,
            CreatedDate = now,
            UpdatedDate = now,
        };

    private static backend.Entities.File MakeFile(string name, string title, string url) =>
        new()
        {
            Name = name,
            Title = title,
            Url = url,
            Description = string.Empty,
        };
}
