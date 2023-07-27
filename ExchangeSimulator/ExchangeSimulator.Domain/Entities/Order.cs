namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// Coin that other players can buy.
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Game Game { get; set; }
    public Guid PlayerCoinId { get; set; }
    public PlayerCoin PlayerCoin { get; set; }

    /// <summary>
    /// Price for one coin.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Max quantity that other players can buy.
    /// </summary>
    public decimal Quantity { get; set; }
}