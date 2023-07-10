using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequestes.JoinGame;

/// <summary>
/// handler for joining to game
/// </summary>
public class JoinGameRequestHandler : IRequestHandler<JoinGameRequest> {
    private readonly IUserContextService _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerCoinRepository _playerCoinRepository;

    public JoinGameRequestHandler(IUserContextService userContext, IUserRepository userRepository, IGameRepository gameRepository, IPlayerRepository playerRepository, IPlayerCoinRepository playerCoinRepository) {
        _userContext = userContext;
        _userRepository = userRepository;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _playerCoinRepository = playerCoinRepository;
    }
    public async Task Handle(JoinGameRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;
        var user = await _userRepository.GetUserById(userId);

        if(user is null) {
            throw new NotFoundException("User not found");
        }

        var game = await _gameRepository.GetGameByName(request.GameName);

        if(game is null) {
            throw new NotFoundException("Game not found");
        }

        var player = new Player() {
            Id = Guid.NewGuid(),
            Name = user.Username,
            Money = game.Money,
            GameId = game.Id,
            UserId = userId,
        };

        await _playerRepository.CreatePlayer(player);

        var coins = game.StartingCoins.Select(coin => new PlayerCoin() { 
            Id = Guid.NewGuid(),
            Name = coin.Name,
            Quantity = coin.Quantity,
            PlayerId = player.Id
        });
        await _playerCoinRepository.CraeteCoins(coins);
    }
}

