using ExchangeSimulator.Api.Hubs;
using ExchangeSimulator.Api.Models.Order;
using ExchangeSimulator.Application.Hubs;
using ExchangeSimulator.Application.Requests.OrderRequests.BuyOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOwnerOrders;
using ExchangeSimulator.Application.Requests.OrderRequests.SellOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game/{gameName}/order")]
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
    public async Task<IActionResult> CreateOrder([FromRoute] string gameName, [FromBody] CreateOrderModel model)
    {
        var request = new CreateOrderRequest()
        {
            GameName = gameName,
            PlayerCoinId = model.PlayerCoinId,
            Price = model.Price,
            Quantity = model.Quantity,
            Type = model.Type
        };

        await _mediator.Send(request);
        await _hub.Clients.Groups(gameName).OrdersChanged();
        return Ok();
    }

    [HttpGet]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOrders([FromRoute] string gameName, GetAllOrdersModel model)
    {
        var request = new GetAllOrdersRequest()
        {
            CoinName = model.CoinName,
            ElementsCount = model.ElementsCount,
            GameName = gameName,
            OrderType = model.OrderType
        };
        var orders = await _mediator.Send(request);
        return Ok(orders);
    }

    [HttpGet("owner-orders")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOwnerOrders([FromRoute] string gameName, GetAllOwnerOrdersModel model)
    {
        var request = new GetAllOwnerOrdersRequest
        {
            GameName = gameName,
            OrderType = model.OrderType,
            PageNumber = model.PageNumber,
            PlayerId = model.PlayerId
        };
        var orders = await _mediator.Send(request);
        return Ok(orders);
    }

    [HttpPut("{orderId}/buy")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> BuyOrder([FromRoute] string gameName, [FromRoute] Guid orderId, BuyOrderModel model)
    {
        var request = new BuyOrderRequest
        {
            GameName = gameName,
            OrderId = orderId,
            Quantity = model.Quantity
        };
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }

    [HttpPut("{orderId}/sell")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> SellOrder([FromRoute] string gameName, [FromRoute] Guid orderId, SellOrderModel model)
    {
        var request = new SellOrderRequest
        {
            GameName = gameName,
            OrderId = orderId,
            Quantity = model.Quantity
        };
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }

    [HttpDelete("{orderId}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> DeleteOrder([FromRoute] DeleteOrderRequest request)
    {
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }
}