using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

public interface IStartingCoinRepository {
    Task CreateCoin(StartingCoin coin);
}

