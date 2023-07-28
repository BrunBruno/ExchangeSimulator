using MediatR;

namespace ExchangeSimulator.Application.Requests.PlayerRequests.GetMyPlayer;

public class GetMyPlayerRequest : IRequest<GetMyPlayerDto>
{
    public string GameName { get; set; }
}