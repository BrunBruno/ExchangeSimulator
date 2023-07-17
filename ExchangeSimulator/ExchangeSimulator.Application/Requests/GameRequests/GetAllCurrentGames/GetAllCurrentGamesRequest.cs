using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Pagination.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;

public class GetAllCurrentGamesRequest : IRequest<PagedResult<GetAllCurrentGamesDto>>
{
    public string? GameName { get; set; }
    public string? OwnerName { get; set; }
    public int PageNumber { get; set; } = 1;
    public GameSortOption SortOption { get; set; } = GameSortOption.Date;
}