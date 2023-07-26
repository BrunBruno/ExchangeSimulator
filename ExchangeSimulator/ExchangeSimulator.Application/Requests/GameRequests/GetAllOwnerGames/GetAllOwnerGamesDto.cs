using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;

public class GetAllOwnerGamesDto
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public double PlayersRatio { get; set; }
    public double TimeRatio { get; set; }
    public GameStatus Status { get; set; }

}