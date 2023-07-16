

using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;
public class GetAllCurrentGamesRequestHandler : IRequestHandler<GetAllCurrentGamesRequest, PagedResult<GetAllCurrentGamesDto>> {
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContext;

    public GetAllCurrentGamesRequestHandler(IUserRepository userRepository, IUserContextService userContext) {
        _userRepository = userRepository;
        _userContext = userContext;
    }
    public async Task<PagedResult<GetAllCurrentGamesDto>> Handle(GetAllCurrentGamesRequest request, CancellationToken cancellationToken) {

        var userId = _userContext.GetUserId()!.Value;
        var user = await _userRepository.GetUserById(userId) 
            ?? throw new NotFoundException("User not found.");

        IEnumerable<Game> games = user.Games;


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

        if (request.GameName is not null) {
            games = games.Where(x => x.Name.Contains(request.GameName, StringComparison.OrdinalIgnoreCase));
        }

        if (request.OwnerName is not null) {
            games = games.Where(x => x.Owner.Username.Contains(request.OwnerName, StringComparison.OrdinalIgnoreCase));
        }

        var gameDtos = games.Select(game => new GetAllCurrentGamesDto {
            Name = game.Name,
            Description = game.Description,
            CreatedAt = game.CreatedAt,
            EndGame = game.EndGame,
            AvailableSpots = game.NumberOfPlayers - game.Players.Count,
            OwnerName = game.Owner.Username
        });

        var pagedResult = new PagedResult<GetAllCurrentGamesDto>(gameDtos.ToList(), gameDtos.Count(), 6, request.PageNumber);

        return pagedResult;
    }
}


