using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.StartGame;

/// <summary>
/// Handler for starting the game by the owner.
/// </summary>
public class StartGameRequestHandler : IRequestHandler<StartGameRequest>
{
    private readonly IUserContextService _userContextService;
    private readonly IGameRepository _gameRepository;

    public StartGameRequestHandler(IUserContextService userContextService, IGameRepository gameRepository)
    {
        _userContextService = userContextService;
        _gameRepository = gameRepository;
    }

    public async Task Handle(StartGameRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;
        var game = await _gameRepository.GetGameByName(request.GameName)
                   ?? throw new NotFoundException("Game was not found");

        if (game.OwnerId != userId)
        {
            throw new BadRequestException("Current user is not the owner of this game");
        }

        if (game.Status != GameStatus.Available)
        {
            throw new BadRequestException("Game is not available");
        }

        game.Status = GameStatus.Active;

        await _gameRepository.Update(game);
    }
}