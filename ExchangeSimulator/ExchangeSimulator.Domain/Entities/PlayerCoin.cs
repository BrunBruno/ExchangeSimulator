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
    /// Coins that are being used in orders
    /// </summary>
    public decimal LockedBalance { get; set; } = 0;

    /// <summary>
    /// Turo over on coin
    /// </summary>
    public decimal TurnOver { get; set; } = 0;

    /// <summary>
    /// Player to whom the coin belongs
    /// </summary>
    public Player Player { get; set; }
}