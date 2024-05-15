

using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.FreezeOrder;
public class FreezeOrderRequestHandler : IRequestHandler<FreezeOrderRequest> {
    private readonly IUserContextService _userContextService;
    private readonly IOrderRepository _orderRepository;

    public FreezeOrderRequestHandler(IOrderRepository orderRepository, IUserContextService userContextService) {
        _orderRepository = orderRepository;
        _userContextService = userContextService;
    }
    public async Task Handle(FreezeOrderRequest request, CancellationToken cancellationToken) {
        var userId = _userContextService.GetUserId()!.Value;

        var orderToFreeze = await _orderRepository.GetOrderById(request.OrderId)
            ?? throw new NotFoundException("Order was not found.");


        if (orderToFreeze.PlayerCoin.Player.UserId != userId) {
            throw new UnauthorizedException("You don't own this order");
        }

        if (request.GameName != orderToFreeze.Game.Name) {
            throw new BadRequestException("Order does not exist in this game.");
        }

        if (orderToFreeze.Game.Status != GameStatus.Active) {
            throw new BadRequestException("Game is not active.");
        }

        orderToFreeze.Status = OrderStatus.Freeze;

        await _orderRepository.Update(orderToFreeze);
    }
}

