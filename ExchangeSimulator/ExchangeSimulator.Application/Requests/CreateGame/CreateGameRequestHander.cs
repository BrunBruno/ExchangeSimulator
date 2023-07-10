using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.CreateGame;

/// <summary>
/// Handler for creating new game
/// Creates starting coins
/// </summary>
public class CreateGameRequestHander : IRequestHandler<CreateGameRequest> {
    private readonly IUserContextService _userContextService;
    private readonly IGameRepository _gameRepository;
    private readonly IStartingCoinRepository _startingCoinRepository;

    public CreateGameRequestHander(IUserContextService userContextService, IGameRepository gameRepository, IStartingCoinRepository startingCoinRepository) {
        _userContextService = userContextService;
        _gameRepository = gameRepository;
        _startingCoinRepository = startingCoinRepository;
    }
    public async Task Handle(CreateGameRequest request, CancellationToken cancellationToken) {
        var userId = _userContextService.GetUserId()!.Value;



        var game = new Game() {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            PasswordHash = request.Password,
            Money = request.Money,
            EndGame = request.EndGame,
            NumberOfPlayers = request.NumberOfPlayers,
            OwnerId = userId,
        };

        await _gameRepository.CreateGame(game);

        var coins = request.Coins.Select(coin => new StartingCoin() {
            Id = Guid.NewGuid(),
            Name = coin.Name,
            Quantity = coin.Quantity,
            GameId = game.Id,
        });

        await _startingCoinRepository.CreateCoins(coins);


        
    }
}

