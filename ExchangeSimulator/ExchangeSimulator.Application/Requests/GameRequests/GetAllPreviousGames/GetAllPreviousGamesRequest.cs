using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Pagination;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllPreviousGames;

public class GetAllPreviousGamesRequest : IRequest<PagedResult<GetAllPreviousGamesDto>>
{
    public string? GameName { get; set; }
    public string? OwnerName { get; set; }
    public int PageNumber { get; set; } = 1;
    public GameSortOption SortOption { get; set; } = GameSortOption.Date;
}