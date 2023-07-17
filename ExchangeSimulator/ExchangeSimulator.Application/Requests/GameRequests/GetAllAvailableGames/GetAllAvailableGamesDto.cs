namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;

public class GetAllAvailableGamesDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndGame { get; set; }
    public int AvailableSpots { get; set; }
    public string OwnerName { get; set; }
}