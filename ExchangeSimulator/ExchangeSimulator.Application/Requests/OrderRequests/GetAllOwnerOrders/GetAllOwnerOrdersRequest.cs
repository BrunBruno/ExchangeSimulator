using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Domain.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOwnerOrders;

public class GetAllOwnerOrdersRequest : IRequest<PagedResult<GetAllOwnerOrdersDto>> 
{
    public string GameName { get; set; }
    public Guid PlayerId { get; set; }
    public int PageNumber { get; set; } = 1;
    public OrderType OrderType { get; set; } = OrderType.Buy;
}