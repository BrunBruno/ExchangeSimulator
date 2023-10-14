using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;

public class DeleteOrderRequest : IRequest
{
    public Guid OrderId { get; set; }
    public string GameName { get; set; }
}