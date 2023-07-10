using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// implementation of player coin repository
/// </summary>
public class PlayerCoinRepository : IPlayerCoinRepository {
    private readonly ExchangeSimulatorDbContext _dbContext;

    public PlayerCoinRepository(ExchangeSimulatorDbContext dbContext) {
        _dbContext = dbContext;
    }

    ///<inheritdoc/>
    public async Task CraeteCoins(IEnumerable<PlayerCoin> coins) {
        await _dbContext.PlayerCoins.AddRangeAsync(coins);
        await _dbContext.SaveChangesAsync();
    }
}

