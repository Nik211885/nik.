using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = backend.Entities.File;

namespace backend.Data.EntityConfigurations;

public class FileConfiguration : IEntityTypeConfiguration<backend.Entities.File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("Files");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasMaxLength(20);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(255);
        builder.Property(f => f.Title).IsRequired().HasMaxLength(255);
        builder.Property(f => f.Url).IsRequired().HasMaxLength(2048);
        builder.Property(f => f.Description).HasMaxLength(1000);
        builder.HasMany(f => f.AlbumFiles)
               .WithOne(af => af.File)
               .HasForeignKey(af => af.FileId);
    }
}
