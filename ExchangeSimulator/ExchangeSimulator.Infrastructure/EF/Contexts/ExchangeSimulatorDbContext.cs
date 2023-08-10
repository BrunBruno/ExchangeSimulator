using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Contexts;

/// <summary>
/// DbContext.
/// </summary>
public class ExchangeSimulatorDbContext : DbContext
{
    /// <summary>
    /// DbSet for user entity.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// DbSet for role entity.
    /// </summary>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Email verification codes
    /// </summary>
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

    /// <summary>
    /// Dbset for games
    /// </summary>
    public DbSet<Game> Games { get; set; }

    /// <summary>
    /// Dbset for players
    /// </summary>
    public DbSet<Player> Players { get; set; }

    /// <summary>
    /// Dbset for staring options of coins
    /// </summary>
    public DbSet<StartingCoin> StartingCoins { get; set; }

    /// <summary>
    /// Dbset for players coins
    /// </summary>
    public DbSet<PlayerCoin> PlayerCoins { get; set; }

    /// <summary>
    /// Dbset for orders
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Dbset for transactions
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; }


    public ExchangeSimulatorDbContext (DbContextOptions<ExchangeSimulatorDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var configuration = new DbContextConfiguration();

        modelBuilder.ApplyConfiguration<User>(configuration);
        modelBuilder.ApplyConfiguration<Role>(configuration);
        modelBuilder.ApplyConfiguration<EmailVerificationCode>(configuration);
        modelBuilder.ApplyConfiguration<Game>(configuration);
        modelBuilder.ApplyConfiguration<Player>(configuration);
        modelBuilder.ApplyConfiguration<StartingCoin>(configuration);
        modelBuilder.ApplyConfiguration<PlayerCoin>(configuration);
        modelBuilder.ApplyConfiguration<Order>(configuration);
        modelBuilder.ApplyConfiguration<Transaction>(configuration);
    }
}