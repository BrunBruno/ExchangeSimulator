

using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.UpdateOrder;
public class UpdateOrderRequestHandler : IRequestHandler<UpdateOrderRequest> {
    private readonly IOrderRepository _orderRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IUserContextService _userContext;

    public UpdateOrderRequestHandler(IOrderRepository orderRepository,
        IPlayerRepository playerRepository,
        IUserContextService userContext) {
        _orderRepository = orderRepository;
        _playerRepository = playerRepository;
        _userContext = userContext;
    }
    public async Task Handle(UpdateOrderRequest request, CancellationToken cancellationToken) {
        if (request.Price <= 0 || request.Quantity <= 0) {
            throw new BadRequestException("Incorrect value.");
        }

        var userId = _userContext.GetUserId()!.Value;

        var player = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        var order = await _orderRepository.GetOrderById(request.OrderId) 
            ?? throw new NotFoundException("Order not found.");

        // return assetes from order
        switch (order.Type) {
            case OrderType.Buy:
                player.LockedBalance -= (order.Price * order.Quantity);
                player.TotalBalance += (order.Price * order.Quantity);
                break;
            case OrderType.Sell:
                order.PlayerCoin.LockedBalance -= request.Quantity;
                order.PlayerCoin.TotalBalance += request.Quantity;
                break;
        }

        order.Price = request.Price;
        order.Quantity = request.Quantity;

        // get assets based on new order
        switch (order.Type) {
            case OrderType.Buy:
                player.LockedBalance += (request.Price * request.Quantity);
                player.TotalBalance -= (request.Price * request.Quantity);
                break;
            case OrderType.Sell:
                order.PlayerCoin.LockedBalance += request.Quantity;
                order.PlayerCoin.TotalBalance -= request.Quantity;
                break;
        }

        await _orderRepository.Update(order);
    }
}

