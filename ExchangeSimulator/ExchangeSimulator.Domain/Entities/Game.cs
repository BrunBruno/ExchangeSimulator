

using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Domain.Entities;
public class Game {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Money { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Available;
    public int NumberOfPlayers { get; set; }

    public Guid OwnerId { get; set; }
    public User Owner { get; set; }

    public List<Player> Players { get; set; }
    public List<StartingCoin> StartingCoins { get; set; }

}

