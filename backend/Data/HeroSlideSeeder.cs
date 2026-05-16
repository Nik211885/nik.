using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>Seeds Vietnamese hero carousel slides.</summary>
public static class HeroSlideSeeder
{
    /// <summary>Seeds default slides. Skips if any slide already exists.</summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.HeroSlides.AnyAsync()) return;

        db.HeroSlides.AddRange(
            new HeroSlide
            {
                Title       = "Khám phá Thế giới",
                Description = "Nơi những câu chuyện về du lịch, nhiếp ảnh và phong cách sống hội tụ.",
                ImageUrl    = "/assets/images/img.png",
                OrderIndex  = 1,
                IsActive    = true,
            },
            new HeroSlide
            {
                Title       = "Hành trình và Cảm hứng",
                Description = "Chia sẻ góc nhìn và trải nghiệm từ những chuyến đi khắp nơi trên thế giới.",
                ImageUrl    = "/assets/images/img_1.png",
                OrderIndex  = 2,
                IsActive    = true,
            }
        );

        await db.SaveChangesAsync();
    }
}
