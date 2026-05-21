using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core configuration for the <see cref="WorkExperience"/> entity.</summary>
public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.ToTable("WorkExperiences");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(36);
        builder.Property(x => x.Company).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.TechTags).HasMaxLength(500);
        builder.Property(x => x.Order).HasDefaultValue(0);
        builder.Property(x => x.IsPublished).HasDefaultValue(true);
    }
}
