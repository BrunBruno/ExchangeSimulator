
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOwnerOrders;
public class GetAllOwnerOrdersRequestHandler : IRequestHandler<GetAllOwnerOrdersRequest, PagedResult<GetAllOwnerOrdersDto>> {
    private readonly IGameRepository _gameRepository;

    public GetAllOwnerOrdersRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<PagedResult<GetAllOwnerOrdersDto>> Handle(GetAllOwnerOrdersRequest request, CancellationToken cancellationToken) {
        var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game not found.");

        var orders = game.Orders.Where(x => x.PlayerCoin.PlayerId == request.PlayerId && x.Type == request.OrderType);

        var orderDtos = orders.Select(order => new GetAllOwnerOrdersDto() {
            Id = order.Id,
            Price = order.Price,
            Quantity = order.Quantity,
            Type = order.Type,
            CoinName = order.PlayerCoin.Name,
            CoinImageUrl = order.PlayerCoin.ImageUrl,
        });

        var pagedResult = new PagedResult<GetAllOwnerOrdersDto>(orderDtos.ToList(), orderDtos.Count(), 6, request.PageNumber);

        return pagedResult;

    }
}

