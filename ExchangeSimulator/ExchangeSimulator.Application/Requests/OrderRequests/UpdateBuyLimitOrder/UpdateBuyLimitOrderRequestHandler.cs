

using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.UpdateBuyLimitOrder;

public class UpdateBuyLimitOrderRequestHandler : IRequestHandler<UpdateBuyLimitOrderRequest, Guid> {
    private readonly IUserContextService _userContext;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IOrderRepository _orderRepository;

    public UpdateBuyLimitOrderRequestHandler(IUserContextService userContext, IGameRepository gameRepository, IPlayerRepository playerRepository, IOrderRepository orderRepository) {
        _userContext = userContext;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _orderRepository = orderRepository;
    }
    public async Task<Guid> Handle(UpdateBuyLimitOrderRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;

        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game was not found");

        var buyer = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        var buyerOrder = await _orderRepository.GetOrderById(request.OrderId) 
            ?? throw new NotFoundException("Order not found.");

        if (request.Price <= 0 || request.Quantity <= 0) {
            throw new BadRequestException("Incorrect value.");
        }

        var buyerCoin = buyerOrder.PlayerCoin;

        // return assets
        buyer.LockedBalance -= buyerOrder.Price * buyerOrder.Quantity;
        buyer.TotalBalance += buyerOrder.Price * buyerOrder.Quantity;

        // check if player has enought money
        if (buyer.TotalBalance - request.Quantity * request.Price < 0) {
            throw new BadRequestException("Not enought assets.");
        }

        // lock player money
        buyer.LockedBalance += request.Quantity * request.Price;
        buyer.TotalBalance -= request.Quantity * request.Price;

        decimal coinsQuantityToBuy = request.Quantity;

        // existing sell orders
        var existingSellOrders = game.Orders
            .Where(order =>
                order.Status == OrderStatus.Active &&
                order.Type == OrderType.Sell &&
                order.Price <= request.Price &&
                order.PlayerCoin.Name == buyerCoin.Name &&
                order.PlayerCoin.Id != buyerCoin.Id
            )
            .OrderBy(order => order.Price)
            .ThenByDescending(order => order.CreatedAt);

        Guid realizationId = Guid.NewGuid();

        foreach (var order in existingSellOrders) {
            if (coinsQuantityToBuy == 0) {
                break;
            }

            var transaction = RealizeTransaction(buyer, buyerCoin, order, ref coinsQuantityToBuy, realizationId);
            transaction.GameId = game.Id;

            game.Transactions.Add(transaction);
        }

        // update order
        buyerOrder.Price = request.Price;
        buyerOrder.Quantity = coinsQuantityToBuy;
        buyerOrder.Status = OrderStatus.Active;

        if (buyerOrder.Quantity == 0) {
            buyerOrder.Status = OrderStatus.Freeze;
        }
  
        await _gameRepository.Update(game);

        return realizationId;
    }

    private Transaction RealizeTransaction(Player buyer, PlayerCoin buyerCoin, Order order, ref decimal coinsQuantityToBuy, Guid realizationId) {
        var seller = order.PlayerCoin.Player;
        var sellerCoin = order.PlayerCoin;

        // set price
        decimal price = order.Price;

        // set quantioty
        decimal quantity = 0;

        // order has less than player wants to - get all from order
        if (coinsQuantityToBuy >= order.Quantity) {
            quantity = order.Quantity;

            // freez order
            order.Status = OrderStatus.Freeze;

            // order has more than player wants to - get what left in created order
        } else {
            quantity = coinsQuantityToBuy;
        }



        // update quantity
        coinsQuantityToBuy -= quantity;

        // update order quantity
        order.Quantity -= quantity;



        // remove money from buyer locked balance
        buyer.LockedBalance -= quantity * price;

        // add coin to buyer total balance
        buyerCoin.TotalBalance += quantity;

        // remove coins from seller locked balance in limit order
        sellerCoin.LockedBalance -= quantity;

        // add money to total balance
        seller.TotalBalance += quantity * price;



        // add buyer stats
        buyer.TurnOver += quantity * price;
        buyer.Trades += 1;
        buyer.BuyTrades += 1;
        buyerCoin.TurnOver += quantity;

        // add seller stats
        seller.TurnOver += quantity * price;
        seller.Trades += 1;
        seller.SellTrades += 1;
        sellerCoin.TurnOver += quantity;

        var transaction = new Transaction() {
            CoinName = buyerCoin.Name,
            Quantity = quantity,
            Price = price,
            RealizationId = realizationId,
            OrderType = OrderType.Buy
        };

        return transaction;
    }
}
