

namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// PlayerCoin entity
/// </summary>
public class PlayerCoin : Coin {

    /// <summary>
    /// Player id
    /// </summary>
    public Guid PlayerId { get; set; }

    /// <summary>
    /// Player to whom the coin belongs
    /// </summary>
    public Player Player { get; set; }
}

