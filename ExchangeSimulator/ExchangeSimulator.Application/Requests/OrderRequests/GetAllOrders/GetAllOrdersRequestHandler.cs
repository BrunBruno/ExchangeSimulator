
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
public class GetAllOrdersRequestHandler : IRequestHandler<GetAllOrdersRequest, PagedResult<GetAllOrdersDto>> {
    private readonly IGameRepository _gameRepository;

    public GetAllOrdersRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<PagedResult<GetAllOrdersDto>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken) {
        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game not found.");

        var orders = game.Orders.Where(x => x.Type == request.OrderType);

        if (request.OrderType == OrderType.Buy) {
            orders = orders.OrderByDescending(x => x.Price).ToList();
        } else if (request.OrderType == OrderType.Sell) {
            orders = orders.OrderBy(x => x.Price).ToList();
        }

        var orderDtos = orders.Select(order => new GetAllOrdersDto() {
            Price = order.Price,
            Quantity = order.Quantity,
            OrderType = order.Type,
            CoinName = order.PlayerCoin.Name,
            CoinImage = order.PlayerCoin.ImageUrl
        });

        var pagedResult = new PagedResult<GetAllOrdersDto>(orderDtos.ToList(), orderDtos.Count(), request.ElementsCount, 1);
         
        return pagedResult;
    }
}

