
namespace ExchangeSimulator.Domain.Entities;

public abstract class Coin {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Quantity { get; set; }
}

