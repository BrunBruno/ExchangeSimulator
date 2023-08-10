using ExchangeSimulator.Api.Hubs;
using ExchangeSimulator.Api.Models.Order;
using ExchangeSimulator.Application.Hubs;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateBuyLimitOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateBuyMarketOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateSellLimitOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateSellMarketOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.FreezeOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;
using ExchangeSimulator.Application.Requests.OrderRequests.GetAllOwnerOrders;
using ExchangeSimulator.Application.Requests.OrderRequests.UpdateBuyLimitOrder;
using ExchangeSimulator.Application.Requests.OrderRequests.UpdateSellLimitOrder;
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
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{orderId}/limit-buy")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> CreateBuyLimitOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] LimitOrderModel model) {
        var request = new CreateBuyLimitOrderRequest() {
            GameName = gameName,
            PlayerCoinId = model.PlayerCoinId,
            Price = model.Price,
            Quantity = model.Quantity,
        };

        await _mediator.Send(request);

        await _hub.Clients.Groups(gameName).OrdersChanged();

        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{orderId}/limit-sell")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> CreateSellLimitOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] LimitOrderModel model) {
        var request = new CreateSellLimitOrderRequest() {
            GameName = gameName,
            PlayerCoinId = model.PlayerCoinId,
            Price = model.Price,
            Quantity = model.Quantity,
        };

        await _mediator.Send(request);

        await _hub.Clients.Groups(gameName).OrdersChanged();

        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}/market-buy")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> MakeBuyMarketOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] MarketOrderModel model) {
        var request = new CreateBuyMarketOrderRequest() {
            GameName = gameName,
            PlayerCoinId = model.PlayerCoinId,
            Quantity = model.Quantity,
        };

        await _mediator.Send(request);

        await _hub.Clients.Groups(gameName).OrdersChanged();

        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}/market-sell")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> MakeSellMarketOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] MarketOrderModel model) {
        var request = new CreateSellMarketOrderRequest() {
            GameName = gameName,
            PlayerCoinId = model.PlayerCoinId,
            Quantity = model.Quantity,
        };

        await _mediator.Send(request);

        await _hub.Clients.Groups(gameName).OrdersChanged();

        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}/limit-buy")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> UpdateBuyLimitOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] LimitOrderModel model) {
        var request = new UpdateBuyLimitOrderRequest() {
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
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{orderId}/limit-sell")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> UpdateSellLimitOrder([FromRoute] string gameName, [FromRoute] Guid orderId, [FromBody] LimitOrderModel model) {
        var request = new UpdateSellLimitOrderRequest() {
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
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPatch("{orderId}/freeze")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> FreezeOrder([FromRoute] string gameName, [FromRoute] Guid orderId) {
        var request = new FreezeOrderRequest() {
            GameName = gameName,
            OrderId = orderId
        };

        await _mediator.Send(request);

        await _hub.Clients.Groups(request.GameName).OrdersChanged();

        return Ok();
    }

    /// <summary>
    /// deletes order
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpDelete("{orderId}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> DeleteOrder([FromRoute] string gameName, [FromRoute] Guid orderId) {
        var request = new DeleteOrderRequest() {
            GameName = gameName,
            OrderId = orderId
        };

        await _mediator.Send(request);

        await _hub.Clients.Groups(request.GameName).OrdersChanged();

        return Ok();
    }
}