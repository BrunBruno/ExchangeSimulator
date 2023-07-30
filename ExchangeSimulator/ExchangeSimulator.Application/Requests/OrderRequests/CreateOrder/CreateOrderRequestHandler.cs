using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.CreateOrder;

public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest>
{
    private readonly IUserContextService _userContextService;
    private readonly ICoinRepository _coinRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;

    public CreateOrderRequestHandler(ICoinRepository coinRepository,
        IUserContextService userContextService,
        IGameRepository gameRepository,
        IPlayerRepository playerRepository)
    {
        _coinRepository = coinRepository;
        _userContextService = userContextService;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
    }

    public async Task Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var playerCoin = await _coinRepository.GetPlayerCoinById(request.PlayerCoinId)
            ?? throw new NotFoundException("PlayerCoin was not found.");

        var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game was not found");

        var player = await _playerRepository.GetPlayerByUserIdAndGameName(request.GameName, userId)
            ?? throw new NotFoundException("Player not found.");

        if (playerCoin.Player.UserId != userId)
        {
            throw new UnauthorizedException($"User does not own player with id: {playerCoin.PlayerId}");
        }

        if (game.Id != playerCoin.Player.GameId)
        {
            throw new BadRequestException("Player is not in this game");
        }

        if (game.Status != GameStatus.Active)
        {
            throw new BadRequestException("Game is not active.");
        }

        switch (request.Type)
        {
            case OrderType.Buy:
                player.LockedBalance += (request.Price * request.Quantity);
                player.TotalBalance -= (request.Price * request.Quantity);
                break;
            case OrderType.Sell:
                playerCoin.LockedBalance += request.Quantity;
                playerCoin.TotalBalance -= request.Quantity;
                break;
        }

        var newOrder = new Order
        {
            PlayerCoinId = playerCoin.Id,
            Price = request.Price,
            Quantity = request.Quantity,
            Type = request.Type
        };

        //await _playerRepository.Update(player);

        game.Orders.Add(newOrder);
        await _gameRepository.Update(game);
    }
}