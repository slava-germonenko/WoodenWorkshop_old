using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WoodenWorkshop.Core.Models.Configuration;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> modelBuilder)
    {
        modelBuilder
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRole>(
                builder =>
                    builder
                        .HasOne(ur => ur.Role)
                        .WithMany()
                        .HasForeignKey(ur => ur.RoleId),
                builder =>
                    builder
                        .HasOne(ur => ur.User)
                        .WithMany()
                        .HasForeignKey(ur => ur.UserId),
                builder =>
                    builder
                        .HasKey("RoleId", "UserId")
            );
    }
}