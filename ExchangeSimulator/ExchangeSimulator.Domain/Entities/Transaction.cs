
namespace ExchangeSimulator.Domain.Entities;

public class Transaction {
    public Guid Id { get; set; }
    public string CoinName { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime MadeOn { get; set; } = DateTime.UtcNow;

    public Guid GameId { get; set; }
    public Game Game { get; set; }
}

