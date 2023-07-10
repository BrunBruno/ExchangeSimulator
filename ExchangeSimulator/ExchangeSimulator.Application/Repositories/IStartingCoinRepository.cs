using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

/// <summary>
/// Interface for starting coin repository
/// </summary>
public interface IStartingCoinRepository {
    /// <summary>
    /// Adds starting coins
    /// </summary>
    /// <param name="coins"></param>
    /// <returns></returns>
    Task CreateCoins(IEnumerable<StartingCoin> coins);
}

