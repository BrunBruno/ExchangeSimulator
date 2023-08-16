
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.CreateBuyMarketOrder;
public class CreateBuyMarketOrderRequestHandler : IRequestHandler<CreateBuyMarketOrderRequest, Guid> {
    private readonly IUserContextService _userContext;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICoinRepository _coinRepository;

    public CreateBuyMarketOrderRequestHandler(IUserContextService userContext, IGameRepository gameRepository, IPlayerRepository playerRepository, ICoinRepository coinRepository) {
        _userContext = userContext;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _coinRepository = coinRepository;
    }

    public async Task<Guid> Handle(CreateBuyMarketOrderRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;

        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game was not found");

        var buyer = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        var buyerCoin = await _coinRepository.GetPlayerCoinById(request.PlayerCoinId)
            ?? throw new NotFoundException("PlayerCoin was not found.");

        // no check in buy market order of player has enough money
        // no locking balance

        // owener creates buy order
        buyer.CreatedOrders += 1;
        buyer.BuyCreated += 1;

        decimal coinsQuantityToBuy = request.Quantity;
        decimal moneyQuantityToSpend = buyer.TotalBalance;


        // existing sell orders
        var existingSellOrders = game.Orders
            .Where(order =>
                order.Status == OrderStatus.Active &&
                order.Type == OrderType.Sell &&
                order.PlayerCoin.Name == buyerCoin.Name &&
                order.PlayerCoin.Id != buyerCoin.Id
            )
            .OrderBy(order => order.Price)
            .ThenByDescending(order => order.CreatedAt);

        Guid realizationId = Guid.NewGuid();

        foreach (var order in existingSellOrders) {
            if (moneyQuantityToSpend == 0 || coinsQuantityToBuy == 0) {
                break;
            }

            var transaction = RealizeTransaction(buyer, buyerCoin, order, ref coinsQuantityToBuy, ref moneyQuantityToSpend, realizationId);
            transaction.GameId = game.Id;

            game.Transactions.Add(transaction);
        }

        await _gameRepository.Update(game);

        return realizationId;
    }

    private Transaction RealizeTransaction(Player buyer, PlayerCoin buyerCoin, Order order, ref decimal coinsQuantityToBuy, ref decimal moneyQuantityToSpend, Guid realizationId) {
        var seller = order.PlayerCoin.Player;
        var sellerCoin = order.PlayerCoin;

        // set price
        decimal price = order.Price;

        decimal quantity = 0;
        // order has less than player wants to, then tak whole order
        if (moneyQuantityToSpend >= order.Quantity * price) {
            quantity = order.Quantity;

            // freez order
            order.Status = OrderStatus.Freeze;

            // order has more than player wants to
        } else {
            quantity = moneyQuantityToSpend / price;
        }



        // update quantity
        coinsQuantityToBuy -= quantity;
        moneyQuantityToSpend -= quantity * price;

        // update order quantity
        order.Quantity -= quantity;



        // remove money from buyer locked balance
        buyer.TotalBalance -= quantity * price;

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
        };

        return transaction;
    }
}

