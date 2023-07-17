using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.StartGame;

public class StartGameRequest : IRequest
{
    public string GameName { get; set; }
}