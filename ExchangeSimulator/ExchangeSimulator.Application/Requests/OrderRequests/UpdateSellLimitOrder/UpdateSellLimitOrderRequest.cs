
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.UpdateSellLimitOrder;

public class UpdateSellLimitOrderRequest : IRequest<Guid> {

    public string GameName { get; set; }
    public Guid OrderId { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
}

