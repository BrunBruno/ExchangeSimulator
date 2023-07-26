using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;

public class GetAllCurrentGamesDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AvailableSpots { get; set; }
    public double PlayersRatio { get; set; }
    public double TimeRatio { get; set; }
    public string OwnerName { get; set; }

    // Extra status for current games
    public GameStatus Status { get; set; }
}