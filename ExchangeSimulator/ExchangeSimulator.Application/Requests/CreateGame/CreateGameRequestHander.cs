using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.CreateGame;

public class CreateGameRequestHander : IRequestHandler<CreateGameRequest> {
    private readonly IUserContextService _userContextService;
    private readonly IGameRepository _gameRepository;

    public CreateGameRequestHander(IUserContextService userContextService, IGameRepository gameRepository) {
        _userContextService = userContextService;
        _gameRepository = gameRepository;
    }
    public async Task Handle(CreateGameRequest request, CancellationToken cancellationToken) {
        var userId = _userContextService.GetUserId()!.Value;



        var game = new Game() {
            Id = new Guid(),
            Name = request.Name,
            Description = request.Description,
            Money = request.Money,
            Duration = request.Duration,
            NumberOfPlayers = request.NumberOfPlayers,
            OwnerId = userId,
        };

        await _gameRepository.CreateGame(game);


        foreach(var coin in request.Coins) {
            var gameCoin = new StartingCoin() {
                Id = new Guid(),
                Name = coin.Name,
                Quantity = coin.Quantity,
                GameId = game.Id,
            };
        }
    }
}

