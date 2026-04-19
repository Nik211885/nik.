using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class AlbumFileConfiguration : IEntityTypeConfiguration<AlbumFile>
{
    public void Configure(EntityTypeBuilder<AlbumFile> builder)
    {
        builder.ToTable("AlbumFiles");
    }
}
