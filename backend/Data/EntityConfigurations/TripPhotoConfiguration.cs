using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core Fluent API configuration for the <see cref="TripPhoto"/> entity.</summary>
public class TripPhotoConfiguration : IEntityTypeConfiguration<TripPhoto>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<TripPhoto> builder)
    {
        builder.ToTable("TripPhotos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasMaxLength(36);
        builder.Property(p => p.TripId).HasMaxLength(36).IsRequired();
        builder.Property(p => p.Url).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Caption).HasMaxLength(255);
        builder.Property(p => p.Order).HasDefaultValue(0);
        builder.Property(p => p.CreatedDate).IsRequired();

        builder.HasOne(p => p.Trip)
            .WithMany(t => t.TripPhotos)
            .HasForeignKey(p => p.TripId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
