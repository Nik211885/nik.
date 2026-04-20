using backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .IsRequired();
        builder.Property(u=>u.UserName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Email)
            .HasMaxLength(255);
        builder.Property(u => u.Phone)
            .HasMaxLength(20);
        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(u => u.Bio)
            .HasMaxLength(1000);
        builder.Property(u => u.CreatedDate)
            .IsRequired();
        builder.Property(u => u.UpdatedDate)
            .IsRequired();
        builder.Property(u => u.Slug)
            .IsRequired()
            .HasMaxLength(255);
    }
}
