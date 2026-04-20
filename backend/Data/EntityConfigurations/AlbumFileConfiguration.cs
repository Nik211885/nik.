using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class AlbumFileConfiguration : IEntityTypeConfiguration<AlbumFile>
{
    public void Configure(EntityTypeBuilder<AlbumFile> builder)
    {
        builder.ToTable("AlbumFiles");
        builder.HasKey(af => new {af.FileId, af.AlbumId});
        builder.HasOne(af => af.Album)
            .WithMany(a => a.AlbumFiles)
            .HasForeignKey(af => af.AlbumId);
        builder.HasOne(af => af.File)
            .WithMany(f => f.AlbumFiles)
            .HasForeignKey(af => af.FileId);
    }
}
