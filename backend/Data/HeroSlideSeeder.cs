using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>Seeds default hero carousel slides idempotently on startup.</summary>
public static class HeroSlideSeeder
{
    /// <summary>Ensures at least two default slides exist. Skips seeding when any slide already exists.</summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.HeroSlides.AnyAsync()) return;

        db.HeroSlides.AddRange(
            new HeroSlide
            {
                Title = "Discover the Place",
                Description = "Find great places to stay, eat, shop, or visit from local experts",
                ImageUrl = "/assets/images/img.png",
                OrderIndex = 1,
                IsActive = true
            },
            new HeroSlide
            {
                Title = "Explore and Travel",
                Description = "Find great places to stay, eat, shop, or visit from local experts",
                ImageUrl = "/assets/images/img_1.png",
                OrderIndex = 2,
                IsActive = true
            }
        );

        await db.SaveChangesAsync();
    }
}
