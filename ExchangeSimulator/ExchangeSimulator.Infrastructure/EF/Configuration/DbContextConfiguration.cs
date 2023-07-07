using ExchangeSimulator.Domain;
using ExchangeSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeSimulator.Infrastructure.EF.Configuration;

/// <summary>
/// Configurations for DbContext.
/// </summary>
public class DbContextConfiguration : IEntityTypeConfiguration<User>, IEntityTypeConfiguration<Role>, IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
       builder
           .HasKey(x => x.Id);
       builder
           .HasOne(x => x.Role)
           .WithMany()
           .HasForeignKey(x => x.RoleId);
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .HasKey(x => x.Id);
        builder
            .HasData(GetRoles());
    }

    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<EmailVerificationCode>(x => x.UserId);
    }

    private IEnumerable<Role> GetRoles()
    {

        var roles = new List<Role>
        {
            new Role()
            {
                Id = (int)Roles.User,
                Name = "User"

            },
            new Role()
            {
                Id = (int)Roles.Admin,
                Name = "Admin"
            }
        };

        return roles;
    }
}