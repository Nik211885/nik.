using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

/// <summary>EF Core configuration for the <see cref="Contact"/> entity.</summary>
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasMaxLength(50);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
        builder.Property(c => c.Subject).IsRequired().HasMaxLength(255);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.CreatedDate).IsRequired();
        builder.Property(c => c.IsRead).HasDefaultValue(false);
    }
}
