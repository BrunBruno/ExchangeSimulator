
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.CreateSellMarketOrder;
public class CreateSellMarketOrderRequestHandler : IRequestHandler<CreateSellMarketOrderRequest, Guid> {
    private readonly IUserContextService _userContext;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICoinRepository _coinRepository;

    public CreateSellMarketOrderRequestHandler(IUserContextService userContext, IGameRepository gameRepository, IPlayerRepository playerRepository, ICoinRepository coinRepository) {
        _userContext = userContext;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _coinRepository = coinRepository;
    }

    public async Task<Guid> Handle(CreateSellMarketOrderRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;

        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game was not found");

        var seller = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        var sellerCoin = await _coinRepository.GetPlayerCoinById(request.PlayerCoinId)
            ?? throw new NotFoundException("PlayerCoin was not found.");

        // check if player has enought coin assets
        if (sellerCoin.TotalBalance - request.Quantity < 0) {
            throw new BadRequestException("Not enought assets");
        }

        // lock player coins
        sellerCoin.LockedBalance += request.Quantity;
        sellerCoin.TotalBalance -= request.Quantity;

        // owener creates sell order
        seller.CreatedOrders += 1;
        seller.SellCreated += 1;

        var coinsQuantityToSell = request.Quantity;

        // existing buy orders
        var existingBuyOrders = game.Orders
          .Where(order =>
              order.Status == OrderStatus.Active &&
              order.Type == OrderType.Buy &&
              order.PlayerCoin.Name == sellerCoin.Name &&
              order.PlayerCoin.Id != sellerCoin.Id
          )
          .OrderByDescending(order => order.Price)
          .ThenByDescending(order => order.CreatedAt);

        Guid realizationId = Guid.NewGuid();

        foreach (var order in existingBuyOrders) {
            if (coinsQuantityToSell == 0) {
                break;
            }

            var transaction = RealizeTransaction(seller, sellerCoin, order, ref coinsQuantityToSell, realizationId);
            transaction.GameId = game.Id;

            game.Transactions.Add(transaction);
        }

        // add new market sell order

        await _gameRepository.Update(game);

        return realizationId;
    }

    private Transaction RealizeTransaction(Player seller, PlayerCoin sellerCoin, Order order, ref decimal coinsQuantityToSell, Guid realizationId) {
        var buyer = order.PlayerCoin.Player;
        var buyerCoin = order.PlayerCoin;

        // set quantity
        decimal quantity = 0;
        // order has less than player wants to
        if (coinsQuantityToSell >= order.Quantity) {
            quantity = order.Quantity;

            // freez order
            order.Status = OrderStatus.Freeze;

            // order has more than player wants to
        } else {
            quantity = coinsQuantityToSell;
        }

        // set price
        decimal price = order.Price;

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


        var transaction = new Transaction() {
            CoinName = sellerCoin.Name,
            Quantity = quantity,
            Price = price,
            RealizationId = realizationId
        };

        return transaction;
    }
}
