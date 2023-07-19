using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;
public class GetAllAvailableGamesRequestHandler : IRequestHandler<GetAllAvailableGamesRequest, PagedResult<GetAllAvailableGamesDto>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserContextService _userContext;

    public GetAllAvailableGamesRequestHandler(IGameRepository gameRepository, IUserContextService userContext)
    {
        _gameRepository = gameRepository;
        _userContext = userContext;
    }
    public async Task<PagedResult<GetAllAvailableGamesDto>> Handle(GetAllAvailableGamesRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId()!.Value;

        var userGames = await _gameRepository.GetGamesByUserId(userId)
                        ?? throw new NotFoundException("User not found.");

        var availableGames = await _gameRepository.GetAllGamesByStatus(GameStatus.Available);

        var games = availableGames.Except(userGames);

        switch (request.SortOption)
        {
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
            AvailableSpots = game.NumberOfPlayers - game.Players.Count,
            OwnerName = game.Owner.Username
        });

        var pagedResult = new PagedResult<GetAllAvailableGamesDto>(gameDtos.ToList(), gameDtos.Count(), 6, request.PageNumber);

        return pagedResult;
    }
}