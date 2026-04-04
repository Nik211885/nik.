using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class TranslateConfiguration : IEntityTypeConfiguration<Translate>
{
    public void Configure(EntityTypeBuilder<Translate> builder)
    {
        builder.ToTable("Translates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(20);
        builder.Property(x => x.CodeId).HasMaxLength(20);
        builder.Property(x => x.LanguageId).HasMaxLength(20);
        builder.Property(x => x.Value).HasMaxLength(300).IsRequired();
        builder.HasOne(x => x.CodeLanguage)
            .WithMany(x => x.Translates)
            .HasForeignKey(x => x.CodeId);
        builder.HasOne(x=>x.Language)
            .WithMany(x=>x.Translates)
            .HasForeignKey(x => x.LanguageId);
    }
}
