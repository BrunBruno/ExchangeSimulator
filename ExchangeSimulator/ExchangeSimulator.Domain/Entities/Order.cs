using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// Coin that other players can buy.
/// </summary>
public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Price for one coin.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Max quantity that other players can buy/sell.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Determines if order is for buying or selling.
    /// </summary>
    public OrderType Type { get; set; }

    /// <summary>
    /// Status of order
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Active;

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Game id
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Game in which order was created
    /// </summary>
    public Game Game { get; set; }

    /// <summary>
    /// Coin id
    /// </summary>
    public Guid PlayerCoinId { get; set; }

    /// <summary>
    /// Coin used in order
    /// </summary>
    public PlayerCoin PlayerCoin { get; set; }
}