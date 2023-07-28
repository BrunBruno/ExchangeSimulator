using ExchangeSimulator.Application.Requests.PlayerRequests.GetMyPlayer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetMyPlayer([FromQuery] GetMyPlayerRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}