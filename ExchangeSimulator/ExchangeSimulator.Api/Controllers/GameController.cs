using ExchangeSimulator.Application.Requests.GameRequestes.CreateGame;
using ExchangeSimulator.Application.Requests.GameRequestes.JoinGame;
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

    /// <summary>
    /// Joins player to game
    /// Creates new player
    /// Create Player coins list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("join-game")]
    public async Task<IActionResult> JoinGame(JoinGameRequest request) {
        await _mediator.Send(request);
        return Ok();
    }
}

