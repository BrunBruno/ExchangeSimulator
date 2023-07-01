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

    public ExchangeSimulatorDbContext (DbContextOptions<ExchangeSimulatorDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var configuration = new DbContextConfiguration();

        modelBuilder.ApplyConfiguration<User>(configuration);
        modelBuilder.ApplyConfiguration<Role>(configuration);
    }
}