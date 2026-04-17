using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class SysConfigEntityConfiguration : IEntityTypeConfiguration<SysConfig>
{
    public void Configure(EntityTypeBuilder<SysConfig> builder)
    {
        builder.ToTable("SysConfigs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(20);
        builder.Property(x => x.Key)
            .HasMaxLength(50);
        builder.Property(x => x.Value)
            .HasColumnType("jsonb");
    }
}
