using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <inheritdoc/>
public class WallMessageReactionConfiguration : IEntityTypeConfiguration<WallMessageReaction>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<WallMessageReaction> builder)
    {
        builder.ToTable("WallMessageReactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(36);
        builder.Property(x => x.WallMessageId).IsRequired().HasMaxLength(36);
        builder.Property(x => x.DeviceId).IsRequired().HasMaxLength(36);
        builder.Property(x => x.CreatedDate).IsRequired();

        builder.HasIndex(x => new { x.WallMessageId, x.DeviceId }).IsUnique();

        builder.HasOne(x => x.WallMessage)
               .WithMany()
               .HasForeignKey(x => x.WallMessageId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
