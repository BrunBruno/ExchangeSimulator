

using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.ChangeOrderStatus;
public class ChangeOrderStatusRequest : IRequest {
    public string GameName  { get; set; }
    public Guid OrderId { get; set; }
}

