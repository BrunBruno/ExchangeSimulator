
namespace ExchangeSimulator.Domain.Entities;

public class Player {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Money { get; set; }

    public int TradesQuantity { get; set; } = 0;
    public int TurnOver { get; set; } = 0;


    public Guid GameId { get; set; }
    public Game Game { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public List<PlayerCoin> PlayerCoins { get; set; }
}

