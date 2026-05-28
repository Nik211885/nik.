using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core Fluent API configuration for the <see cref="Province"/> entity.</summary>
public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Provinces");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasMaxLength(36);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(100).IsRequired();

        builder.HasMany(p => p.Trips)
            .WithOne(t => t.Province)
            .HasForeignKey(t => t.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
