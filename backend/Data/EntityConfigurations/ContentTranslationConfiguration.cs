using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core configuration for <see cref="ContentTranslation"/>.</summary>
public class ContentTranslationConfiguration : IEntityTypeConfiguration<ContentTranslation>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ContentTranslation> builder)
    {
        builder.ToTable("ContentTranslations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(36);
        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.EntityId).IsRequired().HasMaxLength(36);
        builder.Property(x => x.Field).IsRequired().HasMaxLength(50);
        builder.Property(x => x.LangCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Value).IsRequired();

        // Primary lookup: single entity translations
        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.LangCode });

        // Secondary lookup: batch queries and status list
        builder.HasIndex(x => new { x.EntityType, x.LangCode });

        // Prevent duplicate rows on concurrent upserts
        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.Field, x.LangCode }).IsUnique();
    }
}
