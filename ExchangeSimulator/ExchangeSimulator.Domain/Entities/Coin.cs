
namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// coin entity
/// </summary>
public abstract class Coin {

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Coin name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Amount of coins
    /// </summary>
    public decimal TotalBalance { get; set; }

    /// <summary>
    /// Coinis that are being used in orders
    /// </summary>
    public decimal LockedBalace { get; set; } = 0;

    /// <summary>
    /// Image of coin
    /// </summary>
    public string? ImageUrl { get; set; }
}

