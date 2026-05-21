using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <inheritdoc/>
public class WallMessageConfiguration : IEntityTypeConfiguration<WallMessage>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<WallMessage> builder)
    {
        builder.ToTable("WallMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(36);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Status).IsRequired().HasDefaultValue(WallMessageStatus.Pending);
        builder.Property(x => x.Source).HasMaxLength(200);
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.Property(x => x.CreatedDate).IsRequired();
    }
}
