using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

public interface IPlayerCoinRepository {
    Task CraeteCoin(PlayerCoin coin);
}

