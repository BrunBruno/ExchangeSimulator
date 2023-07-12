

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllGames;
public class GameDto {
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndGame { get; set; }
    public int AvilableSpots { get; set; }
    public string ownerName { get; set; }
}

