using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeSimulator.Infrastructure.EF.Configuration;

/// <summary>
/// Configurations for DbContext.
/// </summary>
public class DbContextConfiguration : 
    IEntityTypeConfiguration<User>, 
    IEntityTypeConfiguration<Role>, 
    IEntityTypeConfiguration<EmailVerificationCode>, 
    IEntityTypeConfiguration<Game>, 
    IEntityTypeConfiguration<Player>, 
    IEntityTypeConfiguration<StartingCoin>, 
    IEntityTypeConfiguration<PlayerCoin>,
    IEntityTypeConfiguration<Order>{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(x => x.Id);
        builder
            .HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);
        builder
            .HasMany(x => x.Games)
            .WithMany();
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

    public void Configure(EntityTypeBuilder<Game> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId);
    }

    public void Configure(EntityTypeBuilder<Player> builder) {
        builder
          .HasKey(x => x.Id);
        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.GameId);
        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }

    public void Configure(EntityTypeBuilder<StartingCoin> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.StartingCoins)
            .HasForeignKey(x => x.GameId);
    }

    public void Configure(EntityTypeBuilder<PlayerCoin> builder) {
        builder
          .HasKey(x => x.Id);
        builder
            .HasOne(x => x.Player)
            .WithMany(x => x.PlayerCoins)
            .HasForeignKey(x => x.PlayerId);
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

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasKey(x => x.Id);
        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.GameId);
        builder
            .HasOne(x => x.PlayerCoin)
            .WithOne()
            .HasForeignKey<Order>(x => x.PlayerCoinId);
    }
}