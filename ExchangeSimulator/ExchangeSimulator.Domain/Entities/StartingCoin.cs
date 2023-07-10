
namespace ExchangeSimulator.Domain.Entities;

public class StartingCoin : Coin {
    public Guid GameId { get; set; }
    public Game Game { get; set; }
}

