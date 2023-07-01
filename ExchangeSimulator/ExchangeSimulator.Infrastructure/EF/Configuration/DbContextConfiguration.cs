using ExchangeSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeSimulator.Infrastructure.EF.Configuration;

/// <summary>
/// Configurations for DbContext.
/// </summary>
public class DbContextConfiguration : IEntityTypeConfiguration<User>, IEntityTypeConfiguration<Role>
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

    private IEnumerable<Role> GetRoles()
    {

        var roles = new List<Role>
        {
            new Role()
            {
                Id = 1,
                Name = "User"

            },
            new Role()
            {
                Id = 2,
                Name = "Admin"
            }
        };

        return roles;
    }
}