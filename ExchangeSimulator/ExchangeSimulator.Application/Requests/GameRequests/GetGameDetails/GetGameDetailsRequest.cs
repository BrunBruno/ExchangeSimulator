
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
public class GetGameDetailsRequest : IRequest<GameDto> {
    public string GameName { get; set; }
}

