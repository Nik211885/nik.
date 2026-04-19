using Microsoft.EntityFrameworkCore;
using backend.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = backend.Entities.File;

namespace backend.Data.EntityConfigurations;

public class FileConfiguration : IEntityTypeConfiguration<backend.Entities.File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("Files");
    }
}
