

using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.ChangeOrderStatus;
public class ChangeOrderStatusRequestHandler : IRequestHandler<ChangeOrderStatusRequest> {
    private readonly IOrderRepository _orderRepository;

    public ChangeOrderStatusRequestHandler(IOrderRepository orderRepository) {
        _orderRepository = orderRepository;
    }
    public async Task Handle(ChangeOrderStatusRequest request, CancellationToken cancellationToken) {

        var order = await _orderRepository.GetOrderById(request.OrderId) 
            ?? throw new NotFoundException("Order not found");

        if (request.GameName != order.Game.Name) {
            throw new BadRequestException("Order does not exist in this game.");
        }

        if (order.Status == OrderStatus.Freeze && order.Quantity > 0) {
            order.Status = OrderStatus.Active;
        } else if (order.Status == OrderStatus.Active) { 
            order.Status = OrderStatus.Freeze;
        }

        await _orderRepository.Update(order);
    }
}

