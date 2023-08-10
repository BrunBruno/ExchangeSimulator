
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Repositories;

/// <summary>
/// Interface for game repository
/// </summary>
public interface IGameRepository {
    /// <summary>
    /// Adds game
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    Task CreateGame(Game game);

    /// <summary>
    /// Gets game by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Game?> GetGameById(Guid id);

    /// <summary>
    /// Gets game by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<Game?> GetGameByName(string name);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<IEnumerable<Game>> GetAllGamesByStatus(GameStatus status);

    /// <summary>
    /// Gets all games that user has joined.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<Game>> GetGamesByUserId(Guid userId);

    /// <summary>
    /// Gets all games that user has created.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<Game>> GetOwnedGamesByUserId(Guid userId);

    /// <summary>
    /// Gets all games where end date exceeded date time now.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Game>> GetGamesToFinish();

    /// <summary>
    /// Updates games
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    Task Update(Game game);
}