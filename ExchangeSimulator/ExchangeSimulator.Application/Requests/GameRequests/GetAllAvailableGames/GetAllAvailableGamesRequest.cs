using ExchangeSimulator.Application.Pagination;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;

public class GetAllAvailableGamesRequest : IRequest<PagedResult<GetAllAvailableGamesDto>> 
{
    public string? GameName { get; set; }
    public string? OwnerName { get; set; }
    public int PageNumber { get; set; } = 1;
}