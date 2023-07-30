using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
public class GetAllOrdersRequestHandler : IRequestHandler<GetAllOrdersRequest, PagedResult<GetAllOrdersDto>> {
    private readonly IGameRepository _gameRepository;
    private readonly IUserContextService _userContext;
    private readonly IPlayerRepository _playerRepository;

    public GetAllOrdersRequestHandler(IGameRepository gameRepository, IUserContextService userContext, IPlayerRepository playerRepository) {
        _gameRepository = gameRepository;
        _userContext = userContext;
        _playerRepository = playerRepository;
    }
    public async Task<PagedResult<GetAllOrdersDto>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken) 
    {
        var userId = _userContext.GetUserId()!.Value;

        var player = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game not found.");

        var orders = game.Orders.Where(x => x.Type == request.OrderType && x.PlayerCoin.PlayerId != player.Id);

        if (request.CoinName is not null) 
        {
            orders = orders.Where(x => x.PlayerCoin.Name == request.CoinName);
        }

        switch (request.OrderType)
        {
            case OrderType.Buy:
                orders = orders.OrderByDescending(x => x.Price).ToList();
                break;
            case OrderType.Sell:
                orders = orders.OrderBy(x => x.Price).ToList();
                break;
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