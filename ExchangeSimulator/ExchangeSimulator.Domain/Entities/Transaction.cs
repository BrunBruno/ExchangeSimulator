
using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Domain.Entities;

public class Transaction {
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Coin used in transaction
    /// </summary>
    public string CoinName { get; set; }

    /// <summary>
    /// Amount of transaction
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Price of order
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime MadeOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Game id
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Game
    /// </summary>
    public Game Game { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid RealizationId { get; set; }
}