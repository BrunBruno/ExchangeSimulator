using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
public class GetGameDetailsDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal TotalBalance { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public GameStatus Status { get; set; }
    public int TotalPlayers { get; set; }
    public int AvailableSpots { get; set; }
    public int PlayerCount { get; set; }
    public List<PlayerDto> Players { get; set; }
    public List<CoinDto> Coins { get; set; }
}

public class PlayerDto {
    public string Name { get; set; }
    public string? imageUrl { get; set; }
}

public class CoinDto {
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
}