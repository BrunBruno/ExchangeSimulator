using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;

public class DeleteOrderRequestHandler : IRequestHandler<DeleteOrderRequest>
{
    private readonly IUserContextService _userContextService;
    private readonly IOrderRepository _orderRepository;
    private readonly IPlayerRepository _playerRepository;

    public DeleteOrderRequestHandler(IOrderRepository orderRepository, IUserContextService userContextService, IPlayerRepository playerRepository)
    {
        _orderRepository = orderRepository;
        _userContextService = userContextService;
        _playerRepository = playerRepository;
    }

    public async Task Handle(DeleteOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var orderToDelete = await _orderRepository.GetOrderById(request.OrderId)
            ?? throw new NotFoundException("Order was not found.");

        var orderOwner = await _playerRepository.GetPlayerByOrderId(request.OrderId) 
            ?? throw new NotFoundException("Player was not found.");

        if (orderToDelete.PlayerCoin.Player.UserId != userId)
        {
            throw new UnauthorizedException("You don't own this order");
        }

        if (request.GameName != orderToDelete.Game.Name)
        {
            throw new BadRequestException("Order does not exist in this game.");
        }

        if (orderToDelete.Game.Status != GameStatus.Active)
        {
            throw new BadRequestException("Game is not active.");
        }

        var coinToUpdate = orderOwner.PlayerCoins.First(x => x.Id == orderToDelete.PlayerCoinId);

        switch (orderToDelete.Type)
        {
            case OrderType.Buy:
                orderOwner.LockedBalance -= (orderToDelete.Price * orderToDelete.Quantity);
                orderOwner.TotalBalance += (orderToDelete.Price * orderToDelete.Quantity);
                break;
            case OrderType.Sell:
                coinToUpdate.LockedBalance -= orderToDelete.Quantity;
                coinToUpdate.TotalBalance += orderToDelete.Quantity;
                break;
        }

        await _orderRepository.Delete(orderToDelete);
        await _playerRepository.Update(orderOwner);
    }
}