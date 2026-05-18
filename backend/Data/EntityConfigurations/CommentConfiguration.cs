using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedDate)
            .IsRequired();
        builder.Property(c=>c.Text)
            .IsRequired()
            .HasMaxLength(1000);
        builder.HasOne(c => c.Article)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.ArticleId);
        builder.Property(c => c.GuestName).HasMaxLength(100);
        builder.Property(c => c.GuestEmail).HasMaxLength(255);
        builder.Property(c => c.GuestWebsite).HasMaxLength(500);
        builder.HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .IsRequired(false);
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
