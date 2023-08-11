﻿using ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game/{gameName}/transaction")]
public class TansactionController : ControllerBase {

    private readonly IMediator _mediator;

    public TansactionController(IMediator mediator) {
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
}

