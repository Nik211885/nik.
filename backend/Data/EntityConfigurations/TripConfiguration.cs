using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core Fluent API configuration for the <see cref="Trip"/> entity.</summary>
public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.ProvinceId).HasMaxLength(36).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(255).IsRequired();
        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.Story).HasColumnType("text");
        builder.Property(t => t.CreatedDate).IsRequired();
        builder.Property(t => t.UpdatedDate).IsRequired();

        builder.HasOne(t => t.Province)
            .WithMany(p => p.Trips)
            .HasForeignKey(t => t.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
