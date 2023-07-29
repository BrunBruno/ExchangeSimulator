using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulator.Application.Requests.GameRequests.JoinGame;

/// <summary>
/// handler for joining to game
/// </summary>
public class JoinGameRequestHandler : IRequestHandler<JoinGameRequest> 
{
    private readonly IUserContextService _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher<Game> _passwordHasher;

    public JoinGameRequestHandler(IUserContextService userContext,
        IUserRepository userRepository, 
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        IPasswordHasher<Game> passwordHasher) {

        _userContext = userContext;
        _userRepository = userRepository;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task Handle(JoinGameRequest request, CancellationToken cancellationToken) 
    {
        var userId = _userContext.GetUserId()!.Value;

        var user = await _userRepository.GetUserById(userId) 
            ?? throw new NotFoundException("User not found");

        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game not found");

        if (game.Status != GameStatus.Available)
        {
            throw new BadRequestException("Game is not available");
        }

        var result = _passwordHasher.VerifyHashedPassword(game, game.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed) {
            throw new BadRequestException("Invalid password");
        }

        var isPlayerInList = game.Players.Any(x => x.UserId == userId);

        if (isPlayerInList) {
            throw new BadRequestException("Player already in game.");
        }

        var availableSpots = game.TotalPlayers - (game.Players.Count + 1);

        if (availableSpots < 0) {
            throw new BadRequestException("Game is full.");
        }

        var player = new Player 
        {
            Id = Guid.NewGuid(),
            Name = user.Username,
            TotalBalance = game.StartingBalance,
            GameId = game.Id,
            UserId = userId
        };

        player.PlayerCoins = game.StartingCoins.Select(coin => new PlayerCoin
        {
            Id = Guid.NewGuid(),
            Name = coin.Name,
            TotalBalance = coin.TotalBalance,
            ImageUrl = coin.ImageUrl,
            PlayerId = player.Id
        }).ToList();

        await _playerRepository.CreatePlayer(player);

        user.Games.Add(game);

        await _userRepository.Update(user);
    }
}