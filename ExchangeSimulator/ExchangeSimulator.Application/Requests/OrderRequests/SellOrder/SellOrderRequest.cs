using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Application.Requests.OrderRequests.SellOrder;

public class SellOrderRequest : IRequest
{
    public Guid OrderId { get; set; }
    public decimal Quantity { get; set; }
    public string GameName { get; set; }
}