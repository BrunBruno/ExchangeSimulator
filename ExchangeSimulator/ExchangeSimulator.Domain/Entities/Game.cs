using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// Game entity
/// </summary>
public class Game {

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of game
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of game
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Password for joining th game
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Amount of money that all players recived at the begining of game
    /// </summary>
    public decimal Money { get; set; }

    /// <summary>
    /// Duration of game
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Start date of the game
    /// </summary>
    public DateTime? StartsAt { get; set; }

    /// <summary>
    /// End date of game
    /// </summary>
    public DateTime? EndsAt { get; set; }

    /// <summary>
    /// Current status of game (active, available, finished)
    /// </summary>
    public GameStatus Status { get; set; } = GameStatus.Available;

    /// <summary>
    /// Amount of players that can join the game
    /// </summary>
    public int NumberOfPlayers { get; set; }

    /// <summary>
    /// Owenrs Id
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Owner of the game
    /// </summary>
    public User Owner { get; set; }

    /// <summary>
    /// List of player that joined game
    /// </summary>
    public List<Player> Players { get; set; }

    /// <summary>
    /// Coins used in game
    /// </summary>
    public List<StartingCoin> StartingCoins { get; set; }

}

