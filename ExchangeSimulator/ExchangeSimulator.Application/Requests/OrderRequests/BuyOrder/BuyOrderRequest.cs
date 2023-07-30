using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.BuyOrder;

public class BuyOrderRequest : IRequest
{
    public Guid OrderId { get; set; }
    public decimal Quantity { get; set; }
    public string GameName { get; set; }
}