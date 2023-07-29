
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
public class GetAllOrdersRequestHandler : IRequestHandler<GetAllOrdersRequest, PagedResult<GetAllOrdersDto>> {
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersRequestHandler(IOrderRepository orderRepository) {
        _orderRepository = orderRepository;
    }
    public async Task<PagedResult<GetAllOrdersDto>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken) {
        var orders = await _orderRepository.GetAllOrdersByType(request.OrderType);

        var orderDtos = orders.Select(order => new GetAllOrdersDto() {
            Price = order.Price,
            Quantity = order.Quantity,
            OrderType = order.Type,
            CoinName = order.PlayerCoin.Name,
            CoinImage = order.PlayerCoin.ImageUrl
        });

        var pagedResult = new PagedResult<GetAllOrdersDto>(orderDtos.ToList(), orderDtos.Count(), 10, request.PageNumber);

        return pagedResult;
    }
}

