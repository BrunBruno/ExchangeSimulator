
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.CreateBuyMarketOrder;

public class CreateBuyMarketOrderRequest : IRequest<Guid> {
    public string GameName { get; set; }
    public Guid PlayerCoinId { get; set; }
    public decimal Quantity { get; set; }
}

