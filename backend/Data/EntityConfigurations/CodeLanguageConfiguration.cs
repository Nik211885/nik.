using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class CodeLanguageConfiguration : IEntityTypeConfiguration<CodeLanguage>
{
    public void Configure(EntityTypeBuilder<CodeLanguage> builder)
    {
        builder.ToTable("CodeLanguages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(20);
        builder.Property(x=>x.Code).IsRequired().HasMaxLength(50);
    }
}
