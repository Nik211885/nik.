using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("Albums");
        builder.HasKey(a=> a.Id);
        builder.Property(a=>a.Id).HasMaxLength(20);
        builder.Property(a => a.Name).HasMaxLength(255).IsRequired();
        builder.Property(a => a.Title).HasMaxLength(255).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1000);
        builder.Property(a => a.Slug).HasMaxLength(255).IsRequired();
        builder.Property(a => a.CountImageRef).HasDefaultValue(0);
        builder.Property(a=>a.CreatedDate).IsRequired();
        builder.Property(a=>a.UpdatedDate).IsRequired();
        builder.HasOne(a => a.File)
            .WithMany()
            .HasForeignKey(a => a.FileDescriptionId);
        builder.HasOne(a => a.ParentAlbum)
            .WithMany(a => a.ChildrenAlbum)
            .HasForeignKey(a => a.AlbumId);
        builder.HasMany(a => a.AlbumFiles)
            .WithOne(af => af.Album)
            .HasForeignKey(af => af.AlbumId);

    }
}
