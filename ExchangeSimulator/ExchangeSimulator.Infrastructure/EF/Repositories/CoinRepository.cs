using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

public class CoinRepository : ICoinRepository
{
    private readonly ExchangeSimulatorDbContext _dbContext;

    public CoinRepository(ExchangeSimulatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlayerCoin?> GetPlayerCoinById(Guid id)
        => await _dbContext.PlayerCoins
            .Include(x => x.Player)
            .FirstOrDefaultAsync(x => x.Id == id);
}