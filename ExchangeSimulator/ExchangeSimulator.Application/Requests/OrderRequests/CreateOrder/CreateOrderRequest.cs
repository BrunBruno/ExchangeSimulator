using ExchangeSimulator.Domain.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.CreateOrder;

public class CreateOrderRequest : IRequest
{
    public string GameName { get; set; }
    public Guid PlayerCoinId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public OrderType Type { get; set; }
}