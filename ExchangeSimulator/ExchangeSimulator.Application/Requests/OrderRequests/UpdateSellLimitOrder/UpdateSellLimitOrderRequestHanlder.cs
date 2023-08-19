
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.UpdateSellLimitOrder;

public class UpdateSellLimitOrderRequestHanlder : IRequestHandler<UpdateSellLimitOrderRequest, Guid> {
    private readonly IUserContextService _userContext;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IOrderRepository _orderRepository;

    public UpdateSellLimitOrderRequestHanlder(IUserContextService userContext, IGameRepository gameRepository, IPlayerRepository playerRepository, IOrderRepository orderRepository) {
        _userContext = userContext;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _orderRepository = orderRepository;
    }
    public async Task<Guid> Handle(UpdateSellLimitOrderRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;

        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game was not found");

        var seller = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        var sellerOrder = await _orderRepository.GetOrderById(request.OrderId)
            ?? throw new NotFoundException("Order not found.");

        if (request.Price <= 0 || request.Quantity <= 0) {
            throw new BadRequestException("Incorrect value.");
        }

        var sellerCoin = sellerOrder.PlayerCoin;

        // return assets
        sellerCoin.LockedBalance -= request.Quantity;
        sellerCoin.TotalBalance += request.Quantity;

        // check if player has enought coin assets
        if (sellerCoin.TotalBalance - request.Quantity < 0) {
            throw new BadRequestException("Not enought assets.");
        }

         // lock player coins
        sellerCoin.LockedBalance += request.Quantity;
        sellerCoin.TotalBalance -= request.Quantity;

        var coinsQuantityToSell = request.Quantity;

        // existing buy orders
        var existingBuyOrders = game.Orders
            .Where(order =>
                order.Status == OrderStatus.Active &&
                order.Type == OrderType.Buy &&
                order.Price >= request.Price &&
                order.PlayerCoin.Name == sellerCoin.Name &&
                order.PlayerCoin.Id != sellerCoin.Id
            )
            .OrderByDescending(order => order.Price)
            .ThenByDescending(order => order.CreatedAt);

        Guid realizationId = Guid.NewGuid();

        // realize order
        foreach (var order in existingBuyOrders) {
            if (coinsQuantityToSell == 0) {
                break;
            }

            var transaction = RealizeTransaction(seller, sellerCoin, order, ref coinsQuantityToSell, realizationId);
            transaction.GameId = game.Id;

            game.Transactions.Add(transaction);
        }

        // update order
        sellerOrder.Price = request.Price;
        sellerOrder.Quantity = coinsQuantityToSell;
        sellerOrder.Status = OrderStatus.Active;
        

        if (sellerOrder.Quantity == 0) {
            sellerOrder.Status = OrderStatus.Freeze;
        }

        await _gameRepository.Update(game);

        return realizationId;
    }

    private Transaction RealizeTransaction(Player seller, PlayerCoin sellerCoin, Order order, ref decimal coinsQuantityToSell, Guid realizationId) {
        var buyer = order.PlayerCoin.Player;
        var buyerCoin = order.PlayerCoin;

        // set price
        decimal price = order.Price;

        // set quantity
        decimal quantity = 0;

        // order has less than player wants to - get all from order
        if (coinsQuantityToSell >= order.Quantity) {
            quantity = order.Quantity;

            // freez order
            order.Status = OrderStatus.Freeze;

            // order has more than player wants to - get what left in created order
        } else {
            quantity = coinsQuantityToSell;
        }



        // update coins to sell
        coinsQuantityToSell -= quantity;

        // update order coins and money quantity
        order.Quantity -= quantity;



        // remove coins from locked balance
        sellerCoin.LockedBalance -= quantity;

        // add money to total balance
        seller.TotalBalance += quantity * price;

        // remove money from locked balance in limit order
        buyer.LockedBalance -= quantity * price;

        // add coin to total balance
        buyerCoin.TotalBalance += quantity;



        // add seller stats
        seller.TurnOver += quantity * price;
        seller.Trades += 1;
        seller.SellTrades += 1;
        sellerCoin.TurnOver += quantity;

        // add buyer stats
        buyer.TurnOver += quantity * price;
        buyer.Trades += 1;
        buyer.BuyTrades += 1;
        buyerCoin.TurnOver += quantity;

        var transaction = new Transaction {
            CoinName = sellerCoin.Name,
            Quantity = quantity,
            Price = price,
            RealizationId = realizationId
        };

        return transaction;
    }
}

