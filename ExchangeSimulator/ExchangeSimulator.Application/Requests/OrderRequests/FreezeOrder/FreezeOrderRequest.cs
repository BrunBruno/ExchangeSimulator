
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.FreezeOrder ;

public class FreezeOrderRequest : IRequest {
    public Guid OrderId { get; set; }
    public string GameName { get; set; }
}
