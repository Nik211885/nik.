using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core configuration for the <see cref="HeroSlide"/> entity.</summary>
public class HeroSlideConfiguration : IEntityTypeConfiguration<HeroSlide>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<HeroSlide> builder)
    {
        builder.ToTable("HeroSlides");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.OrderIndex).HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
