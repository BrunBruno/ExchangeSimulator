
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
public class GetGameDetailsRequest : IRequest<GetGameDetailsDto> {
    public string GameName { get; set; }
}

