using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.ToTable("Reactions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.CreatedDate)
            .IsRequired();
        builder.Property(r=>r.ReactionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.HasOne(r => r.CreatedByUser)
            .WithMany()
            .HasForeignKey(r => r.CreatedByUserId);
        builder.HasOne(r => r.Article)
            .WithMany()
            .HasForeignKey(r => r.ArticleId);
    }
}
