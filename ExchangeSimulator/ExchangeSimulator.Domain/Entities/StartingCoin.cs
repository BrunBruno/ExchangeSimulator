
namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// Starting Coin entity
/// </summary>
public class StartingCoin : Coin {

    /// <summary>
    /// Game id
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Game to which the coin belongs
    /// </summary>
    public Game Game { get; set; }
}

