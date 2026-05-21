using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core configuration for the <see cref="Project"/> entity.</summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(36);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.TechTags).HasMaxLength(500);
        builder.Property(x => x.DemoUrl).HasMaxLength(500);
        builder.Property(x => x.RepoUrl).HasMaxLength(500);
        builder.Property(x => x.Order).HasDefaultValue(0);
        builder.Property(x => x.IsPublished).HasDefaultValue(true);
    }
}
