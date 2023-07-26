using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Pagination.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;

public class GetAllAvailableGamesRequest : IRequest<PagedResult<GetAllAvailableGamesDto>>
{
    public string? GameName { get; set; }
    public string? OwnerName { get; set; }
    public int PageNumber { get; set; } = 1;
    public GameSortOption SortOption { get; set; } = GameSortOption.Date;
}