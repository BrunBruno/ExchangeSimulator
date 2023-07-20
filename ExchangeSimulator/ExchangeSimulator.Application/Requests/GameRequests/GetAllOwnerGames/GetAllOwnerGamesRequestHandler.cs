using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;

public class GetAllOwnerGamesRequestHandler : IRequestHandler<GetAllOwnerGamesRequest, PagedResult<GetAllOwnerGamesDto>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserContextService _userContext;

    public GetAllOwnerGamesRequestHandler(IGameRepository gameRepository, IUserContextService userContext)
    {
        _gameRepository = gameRepository;
        _userContext = userContext;
    }
    public async Task<PagedResult<GetAllOwnerGamesDto>> Handle(GetAllOwnerGamesRequest request, CancellationToken cancellationToken)
    {

        var userId = _userContext.GetUserId()!.Value;
        var games = await _gameRepository.GetOwnedGamesByUserId(userId);

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

        var gameDtos = games.Select(game => new GetAllOwnerGamesDto
        {
            Name = game.Name,
            CreatedAt = game.CreatedAt,
            PlayerCount = game.Players.Count,
            AvailableSpots = game.NumberOfPlayers - game.Players.Count,
            Status = game.Status,
        });

        var pagedResult = new PagedResult<GetAllOwnerGamesDto>(gameDtos.ToList(), gameDtos.Count(), 6, request.PageNumber);

        return pagedResult;
    }
}