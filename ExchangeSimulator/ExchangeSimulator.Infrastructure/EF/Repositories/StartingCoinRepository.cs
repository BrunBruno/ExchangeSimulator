using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// implementation of starting coin repository
/// </summary>
public class StartingCoinRepository : IStartingCoinRepository {
    private readonly ExchangeSimulatorDbContext _dbContext;

    public StartingCoinRepository(ExchangeSimulatorDbContext dbContext) {
        _dbContext = dbContext;
    }

    ///<inheritdoc/>
    public async Task CreateCoins(IEnumerable<StartingCoin> coins) {
        await _dbContext.StartingCoins.AddRangeAsync(coins);
        await _dbContext.SaveChangesAsync();
    }
}

