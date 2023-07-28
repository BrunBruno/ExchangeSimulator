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

    public CreateOrderRequestHandler(ICoinRepository coinRepository, IUserContextService userContextService, IGameRepository gameRepository)
    {
        _coinRepository = coinRepository;
        _userContextService = userContextService;
        _gameRepository = gameRepository;
    }

    public async Task Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var playerCoin = await _coinRepository.GetPlayerCoinById(request.PlayerCoinId)
            ?? throw new NotFoundException("PlayerCoin was not found.");

        var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game was not found");

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

        var newOrder = new Order
        {
            PlayerCoinId = playerCoin.Id,
            Price = request.Price,
            Quantity = request.Quantity,
            Type = request.Type
        };
        game.Orders.Add(newOrder);
        await _gameRepository.Update(game);
    }
}