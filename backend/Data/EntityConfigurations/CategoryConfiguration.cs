using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(20);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200); 
        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(x => x.Image)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x=>x.CreatedDate)
            .IsRequired();
        builder.Property(x=>x.UpdatedDate)  
            .IsRequired();
        builder.Property(x=>x.CountRef)
            .HasDefaultValue(0);
        builder.HasMany(x => x.ArticleCategories)
            .WithOne(ac => ac.Category)
            .HasForeignKey(ac => ac.CategoryId);
    }
}
