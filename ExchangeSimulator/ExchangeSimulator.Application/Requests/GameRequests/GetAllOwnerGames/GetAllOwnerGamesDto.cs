using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;

public class GetAllOwnerGamesDto
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PlayerCount { get; set; }
    public int AvailableSpots { get; set; }
    public GameStatus Status { get; set; }

}