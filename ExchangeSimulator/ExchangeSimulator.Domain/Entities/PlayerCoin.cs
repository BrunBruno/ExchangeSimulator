

namespace ExchangeSimulator.Domain.Entities;

public class PlayerCoin : Coin {
    public Guid PlayerId { get; set; }
    public Player Player { get; set; }
}

