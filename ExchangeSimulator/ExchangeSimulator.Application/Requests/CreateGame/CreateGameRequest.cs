

using MediatR;

namespace ExchangeSimulator.Application.Requests.CreateGame;
public class CreateGameRequest : IRequest {
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Money { get; set; }
    public List<StartingCoinItem> Coins { get; set; }
    public TimeSpan Duration { get; set; }
    public int NumberOfPlayers { get; set; }
    
}

