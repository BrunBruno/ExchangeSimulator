using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

public class StartingCoinRepository : IStartingCoinRepository {
    public Task CreateCoin(StartingCoin coin) {
        throw new NotImplementedException();
    }
}

