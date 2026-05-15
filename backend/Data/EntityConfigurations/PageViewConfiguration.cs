using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core configuration for the <see cref="PageView"/> entity.</summary>
public class PageViewConfiguration : IEntityTypeConfiguration<PageView>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PageView> builder)
    {
        builder.ToTable("PageViews");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasMaxLength(50);
        builder.Property(p => p.Path).IsRequired().HasMaxLength(500);
        builder.Property(p => p.IpAddress).IsRequired().HasMaxLength(50);
        builder.Property(p => p.UserAgent).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Browser).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Os).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Referer).HasMaxLength(500);
        builder.Property(p => p.CreatedDate).IsRequired();

        builder.HasIndex(p => p.CreatedDate);
        builder.HasIndex(p => p.IpAddress);
    }
}
