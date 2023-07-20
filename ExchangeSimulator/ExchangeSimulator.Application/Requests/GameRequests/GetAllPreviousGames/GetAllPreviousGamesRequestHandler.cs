using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllPreviousGames;

public class GetAllPreviousGamesRequestHandler : IRequestHandler<GetAllPreviousGamesRequest, PagedResult<GetAllPreviousGamesDto>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserContextService _userContext;

    public GetAllPreviousGamesRequestHandler(IGameRepository gameRepository, IUserContextService userContext)
    {
        _gameRepository = gameRepository;
        _userContext = userContext;
    }
    public async Task<PagedResult<GetAllPreviousGamesDto>> Handle(GetAllPreviousGamesRequest request, CancellationToken cancellationToken)
    {

        var userId = _userContext.GetUserId()!.Value;
        var games = await _gameRepository.GetGamesByUserId(userId);

        games = games.Where(x => x.Status == GameStatus.Finished);

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

        var gameDtos = games.Select(game => new GetAllPreviousGamesDto
        {
            Name = game.Name,
            Description = game.Description,
            CreatedAt = game.CreatedAt,
            AvailableSpots = game.NumberOfPlayers - game.Players.Count,
            OwnerName = game.Owner.Username
        });

        var pagedResult = new PagedResult<GetAllPreviousGamesDto>(gameDtos.ToList(), gameDtos.Count(), 6, request.PageNumber);

        return pagedResult;
    }
}