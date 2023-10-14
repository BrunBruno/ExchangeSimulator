using ExchangeSimulator.Api.Models.Transaction;
using ExchangeSimulator.Application.Requests.TransactionRequests.GetChartData;
using ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;
using ExchangeSimulator.Application.Requests.TransactionRequests.GetRealizedTransactions;
using ExchangeSimulator.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game/{gameName}/transaction")]
public class TransactionController : ControllerBase {

    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator) {
        _mediator = mediator;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <returns></returns>
    [HttpGet("prices")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetPrices([FromRoute] string gameName, [FromQuery] string coinName) {
        var request = new GetPricesRequest() {
            GameName = gameName,
            CoinName = coinName,
        };

        var result = await _mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("realized/{realizationId}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetRealizedTransactions([FromRoute] GetRealizedTransactionsRequest request) {

        var transactions = await _mediator.Send(request);

        return Ok(transactions);
    }

    /// <summary>
    /// Gets chart data for coin.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="gameName"></param>
    /// <returns></returns>
    [HttpGet("chart-data")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetChartData([FromRoute] string gameName, [FromQuery] GetChartDataModel model)
    {
        var request = new GetChartDataRequest()
        {
            CoinName = model.CoinName,
            PeriodOfTime = model.PeriodOfTime,
            GameName = gameName
        };
        var result = await _mediator.Send(request);

        return Ok(result);
    }
}