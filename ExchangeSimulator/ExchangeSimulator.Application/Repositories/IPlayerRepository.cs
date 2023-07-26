using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

/// <summary>
/// Interface for player repository
/// </summary>
public interface IPlayerRepository {
    /// <summary>
    /// Add new player
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    Task CreatePlayer(Player player);
}

