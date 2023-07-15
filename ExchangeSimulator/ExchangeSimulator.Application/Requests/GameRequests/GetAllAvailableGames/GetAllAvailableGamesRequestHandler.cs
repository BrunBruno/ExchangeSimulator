using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;
public class GetAllAvailableGamesRequestHandler : IRequestHandler<GetAllAvailableGamesRequest, PagedResult<GetAllAvailableGamesDto>> 
{
    private readonly IGameRepository _gameRepository;

    public GetAllAvailableGamesRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<PagedResult<GetAllAvailableGamesDto>> Handle(GetAllAvailableGamesRequest request, CancellationToken cancellationToken) {

        var games = await _gameRepository.GetAllGamesByStatus(GameStatus.Available);

        switch (request.SortOption) {
            case GameSortOption.Date:
                games = games.OrderBy(x => x.CreatedAt);
                break;
            case GameSortOption.Name:
                games = games.OrderBy(x => x.Name);
                break;
            case GameSortOption.Owner:
                games = games.OrderBy(x => x.Owner.Username);
                break;
        }

        if (request.GameName is not null)
        {
            games = games.Where(x => x.Name.Contains(request.GameName, StringComparison.OrdinalIgnoreCase));
        }

        if (request.OwnerName is not null)
        {
            games = games.Where(x => x.Owner.Username.Contains(request.OwnerName, StringComparison.OrdinalIgnoreCase));
        }

        var gameDtos = games.Select(game => new GetAllAvailableGamesDto 
        {
            Name = game.Name,
            Description = game.Description,
            CreatedAt = game.CreatedAt,
            EndGame = game.EndGame,
            AvailableSpots = game.NumberOfPlayers - game.Players.Count,
            OwnerName = game.Owner.Username
        });

        var pagedResult = new PagedResult<GetAllAvailableGamesDto>(gameDtos.ToList(), gameDtos.Count(), 6, request.PageNumber);

        return pagedResult;
    }
}