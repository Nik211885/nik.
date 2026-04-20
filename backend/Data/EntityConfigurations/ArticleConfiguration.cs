using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(20);
        builder.Property(x=>x.Title)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x=>x.Description)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(x=>x.Content)
            .IsRequired();
        builder.Property(x=>x.Image)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x=>x.Slug)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x=>x.CreatedDate)
            .IsRequired();
        builder.Property(x=>x.UpdatedDate)
            .IsRequired();
        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId);
        builder.Property(x=>x.CountCommentRef)
            .HasDefaultValue(0);
        builder.Property(x=>x.CountLikeRef)
            .HasDefaultValue(0);
        builder.Property(x=>x.CountSee)
            .HasDefaultValue(0);
        builder.Property(x=>x.CountHeartRef)
            .HasDefaultValue(0);
    }
}
