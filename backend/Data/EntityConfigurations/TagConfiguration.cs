using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(20);
        builder.Property(x=>x.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Slug)
            .HasMaxLength(150);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.Property(x => x.Image)
            .HasMaxLength(250);
        builder.Property(x => x.CountRef)
            .HasDefaultValue(0);
        builder.HasMany(x=>x.ArticleTags)
            .WithOne(x=>x.Tag)
            .HasForeignKey(x=>x.TagId);
    }
}
