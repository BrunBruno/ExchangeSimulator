using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

/// <summary>
/// Interface for player coins repository
/// </summary>
public interface IPlayerCoinRepository {
    /// <summary>
    /// Adds player coins
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    Task CraeteCoin(PlayerCoin coin);
}

