using ExchangeSimulator.Application.Requests.CreateGame;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase {

    private readonly IMediator _mediator;

    public GameController(IMediator mediator) {
        _mediator = mediator;
    }

    /// <summary>
    /// Creaes nw game and list of starting coins
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateGame(CreateGameRequest request) {
        await _mediator.Send(request);
        return Ok();
    }
}

