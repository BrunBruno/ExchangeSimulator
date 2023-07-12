

using ExchangeSimulator.Application.Repositories;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllGames;
public class GetAllGamesRequestHandler : IRequestHandler<GetAllGamesRequest, IEnumerable<GameDto>> {
    private readonly IGameRepository _gameRepository;

    public GetAllGamesRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<IEnumerable<GameDto>> Handle(GetAllGamesRequest request, CancellationToken cancellationToken) {
        var games = await _gameRepository.GetAllGamesByStatus(request.GameStatus);


        var gameDtos = games.Select(game => new GameDto {
            Name = game.Name,
            Description = game.Description,
            CreatedAt = game.CreatedAt,
            EndGame = game.EndGame,
            AvilableSpots = game.NumberOfPlayers - game.Players.Count,
            ownerName = game.Owner.Username
        });

        return gameDtos;
    }
}

