using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

/// <summary>
/// Interface for player repository
/// </summary>
public interface IPlayerRepository 
{
    /// <summary>
    /// Add new player
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    Task CreatePlayer(Player player);

    /// <summary>
    /// Get player.
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Player?> GetPlayerByUserIdAndGameName(string gameName, Guid userId);

    Task Update(Player player);
}