using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Pagination;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;

public class GetAllOwnerGamesRequest : IRequest<PagedResult<GetAllOwnerGamesDto>>
{
    public string? GameName { get; set; }
    public int PageNumber { get; set; } = 1;
    public GameSortOption SortOption { get; set; } = GameSortOption.Date;
}