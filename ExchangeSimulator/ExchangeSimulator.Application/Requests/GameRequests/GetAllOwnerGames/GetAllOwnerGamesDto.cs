using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;

public class GetAllOwnerGamesDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndGame { get; set; }
    public int AvailableSpots { get; set; }
    public string OwnerName { get; set; }
    public GameStatus Status { get; set; }
}