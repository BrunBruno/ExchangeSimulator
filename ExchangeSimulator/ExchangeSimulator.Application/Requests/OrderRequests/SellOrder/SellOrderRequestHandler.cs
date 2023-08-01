using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.SellOrder;

public class SellOrderRequestHandler : IRequestHandler<SellOrderRequest>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserContextService _userContextService;
    private readonly IPlayerRepository _playerRepository;

    public SellOrderRequestHandler(IOrderRepository orderRepository, IUserContextService userContextService, IPlayerRepository playerRepository)
    {
        _orderRepository = orderRepository;
        _userContextService = userContextService;
        _playerRepository = playerRepository;
    }

    public async Task Handle(SellOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var order = await _orderRepository.GetOrderById(request.OrderId)
            ?? throw new NotFoundException("Order was not found.");

        if (order.Type != OrderType.Sell)
        {
            throw new BadRequestException("Order is not for selling");
        }

        if (order.PlayerCoin.Player.UserId == userId)
        {
            throw new BadRequestException("Player cannot buy/sell his own order.");
        }

        var player = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player was not found.");

        if (request.GameName != order.Game.Name)
        {
            throw new BadRequestException("Order does not exist in this game.");
        }

        if (order.Game.Status != GameStatus.Active)
        {
            throw new BadRequestException("Game is not active.");
        }

        order.PlayerCoin.LockedBalance -= request.Quantity;
        order.Quantity -= request.Quantity;
        order.PlayerCoin.Player.TradesQuantity++;
        order.PlayerCoin.Player.SellTrades++;
        order.PlayerCoin.Player.TurnOver += request.Quantity * order.Price;
        order.PlayerCoin.TurnOver += request.Quantity;

        if (order.Quantity < 0 || order.PlayerCoin.LockedBalance < 0)
        {
            throw new BadRequestException("You cannot buy that much.");
        }

        player.TotalBalance -= request.Quantity * order.Price;
        player.TradesQuantity++;
        player.BuyTrades++;
        player.TurnOver += request.Quantity * order.Price;
       


        if (player.TotalBalance < 0)
        {
            throw new BadRequestException("You don't have enough money.");
        }

        var coinToUpdate = player.PlayerCoins.First(x => x.Name == order.PlayerCoin.Name);
        coinToUpdate.TotalBalance += request.Quantity;
        coinToUpdate.TurnOver += request.Quantity;
        order.PlayerCoin.Player.TotalBalance += request.Quantity * order.Price;

        if (order.Quantity == 0) { 
            order.Status = OrderStatus.Freeze;
        }

        await _orderRepository.Update(order);
        await _playerRepository.Update(player);
    }
}