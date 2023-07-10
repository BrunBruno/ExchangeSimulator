using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// implementation of game repository
/// </summary>
public class GameRepository : IGameRepository {
    private readonly ExchangeSimulatorDbContext _dbContext;

    public GameRepository(ExchangeSimulatorDbContext dbContext) {
        _dbContext = dbContext;
    }

    ///<inheritdoc/>
    public async Task CreateGame(Game game) {
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
    }
}

