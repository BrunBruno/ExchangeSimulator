
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.CreateBuyLimitOrder;

public class CreateBuyLimitOrderRequest : IRequest<Guid> {
    public string GameName { get; set; }
    public Guid PlayerCoinId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}

