using ExchangeSimulator.Domain.Enums;
using MediatR;


namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllGames;
public class GetAllGamesRequest : IRequest<IEnumerable<GameDto>> {
    public GameStatus GameStatus { get; set; }
}

