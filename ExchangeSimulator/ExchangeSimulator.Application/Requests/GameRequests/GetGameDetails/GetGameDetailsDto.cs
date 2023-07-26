using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
public class GetGameDetailsDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Money { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public GameStatus Status { get; set; }
    public int NumberOfPlayers { get; set; }
    public int AvailableSpots { get; set; }
    public int PlayerCount { get; set; }
    public List<PlayerDto> Players { get; set; }
    public List<CoinDto> Coins { get; set; }
}