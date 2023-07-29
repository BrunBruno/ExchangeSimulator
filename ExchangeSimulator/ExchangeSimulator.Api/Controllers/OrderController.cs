using ExchangeSimulator.Api.Hubs;
using ExchangeSimulator.Application.Hubs;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<GameHub, IGameHub> _hub;

    public OrderController(IHubContext<GameHub, IGameHub> hub, IMediator mediator)
    {
        _hub = hub;
        _mediator = mediator;
    }

    /// <summary>
    /// Creates the order.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }

    [HttpGet]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrdersRequest request) {
        var orders = await _mediator.Send(request);
        return Ok(orders);
    }
}