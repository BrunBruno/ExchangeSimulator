
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
    public decimal Quantity { get; set; }

    /// <summary>
    /// Image of coin
    /// </summary>
    public string? ImageUrl { get; set; }
}

