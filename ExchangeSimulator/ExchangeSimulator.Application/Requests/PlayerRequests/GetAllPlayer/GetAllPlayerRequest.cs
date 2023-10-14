
using ExchangeSimulator.Application.Pagination;
using MediatR;

namespace ExchangeSimulator.Application.Requests.PlayerRequests.GetAllPlayer;

public class GetAllPlayerRequest : IRequest<PagedResult<GetAllPlayerDto>> {
    public string GameName { get; set; }
    public int ElementsCount { get; set; } = 100;
}

