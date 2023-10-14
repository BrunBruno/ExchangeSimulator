using ExchangeSimulator.Api.Models.Player;
using ExchangeSimulator.Application.Requests.PlayerRequests.GetAllPlayer;
using ExchangeSimulator.Application.Requests.PlayerRequests.GetMyPlayer;
using ExchangeSimulator.Application.Requests.PlayerRequests.JoinToGame;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game/{gameName}/player")]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Joins player to game
    /// Creates new player
    /// Create Player coins list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("join-game")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> JoinToGame([FromRoute] string gameName, [FromBody] JoinToGameModel model) {
        var request = new JoinToGameRequest {
            GameName = gameName,
            Password = model.Password,
        };

        await _mediator.Send(request);

        return Ok();
    }

    /// <summary>
    /// Gets users player from game
    /// </summary>
    /// <param name="gameName"></param>
    /// <returns></returns>
    [HttpGet("my")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetMyPlayer([FromRoute] string gameName)
    {
        var request = new GetMyPlayerRequest { 
            GameName = gameName 
        };

        var result = await _mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// Gets all players from game
    /// </summary>
    /// <param name="gameName"></param>
    /// <returns></returns>
    [HttpGet("all")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllPlayer([FromRoute] string gameName) {
        var request = new GetAllPlayerRequest() {
            GameName = gameName
        };

        var result = await _mediator.Send(request);

        return Ok(result);
    }
}