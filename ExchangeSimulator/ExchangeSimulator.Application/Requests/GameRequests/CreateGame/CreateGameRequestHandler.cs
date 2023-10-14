using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulator.Application.Requests.GameRequests.CreateGame;

/// <summary>
/// Handler for creating new game
/// Creates starting coins
/// </summary>
public class CreateGameRequestHandler : IRequestHandler<CreateGameRequest>
{
    private readonly IUserContextService _userContextService;
    private readonly IGameRepository _gameRepository;
    private readonly IPasswordHasher<Game> _passwordHasher;

    public CreateGameRequestHandler(IUserContextService userContextService,
        IGameRepository gameRepository,
        IPasswordHasher<Game> passwordHasher)
    {
        _userContextService = userContextService;
        _gameRepository = gameRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(CreateGameRequest request, CancellationToken cancellationToken)
    {
        if (request.TotalPlayers < 1)
        {
            throw new BadRequestException("Number of players must be greater or equal to 1.");
        }

        var userId = _userContextService.GetUserId()!.Value;

        var existingGame = await _gameRepository.GetGameByName(request.Name);

        if (existingGame is not null) {
            throw new BadRequestException("Game already exists.");
        }

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            StartingBalance = request.StartingBalance,
            Duration = new TimeSpan(0, request.Duration, 0),
            TotalPlayers = request.TotalPlayers,
            OwnerId = userId
        };

        game.PasswordHash = _passwordHasher.HashPassword(game, request.Password);

        game.StartingCoins = request.Coins.Select(coin => new StartingCoin
        {
            Name = coin.Name,
            TotalBalance = coin.Quantity,
            ImageUrl = coin.ImageUrl,
        }).ToList();

        await _gameRepository.CreateGame(game);
    }
}