using ExchangeSimulator.Api.Hubs;
using ExchangeSimulator.Api.Models.Order;
using ExchangeSimulator.Application.Hubs;
using ExchangeSimulator.Application.Requests.OrderRequests.BuyOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.ChangeOrderStatus;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOwnerOrders;
using ExchangeSimulator.Application.Requests.OrderRequests.SellOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.UpdateOrder;
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

    /// <summary>
    /// Gets all orders from game
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOrders([FromRoute] string gameName, [FromQuery] GetAllOrdersModel model)
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

    /// <summary>
    /// Gets all orders that user created
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("owner-orders")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOwnerOrders([FromRoute] string gameName,[FromQuery] GetAllOwnerOrdersModel model)
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

    /// <summary>
    /// realization of buy order
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}/buy")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> BuyOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] BuyOrderModel model)
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

    /// <summary>
    /// realization of sell order
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}/sell")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> SellOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] SellOrderModel model)
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

    /// <summary>
    /// updates existing order
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> UpdateOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] UpdateOrderModel model) {
        var request = new UpdateOrderRequest(){ 
            GameName = gameName,
            OrderId = orderId,
            Price = model.Price,
            Quantity = model.Quantity
        };
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }

    /// <summary>
    /// delete order
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpDelete("{orderId}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> DeleteOrder([FromRoute] string gameName, [FromRoute] Guid orderId )
    {
        var request = new DeleteOrderRequest() { 
            GameName = gameName,
            OrderId = orderId
        };
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPatch("{orderId}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> ChangeOrderStatus([FromRoute] string gameName, [FromRoute] Guid orderId) {
        var request = new ChangeOrderStatusRequest() {
            GameName = gameName,
            OrderId = orderId
        };
        await _mediator.Send(request);
        await _hub.Clients.Groups(request.GameName).OrdersChanged();
        return Ok();
    }
}