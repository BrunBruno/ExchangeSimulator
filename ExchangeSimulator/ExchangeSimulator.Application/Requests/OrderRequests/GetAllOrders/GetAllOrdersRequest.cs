

using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Domain.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
public class GetAllOrdersRequest : IRequest<PagedResult<GetAllOrdersDto>> {
    public string GameName { get; set; }
    public OrderType OrderType { get; set; }
    public int PageNumber { get; set; } = 1;
}

