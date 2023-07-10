using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

public class GameRepository : IGameRepository {
    public Task CreateGame(Game game) {
        throw new NotImplementedException();
    }
}

