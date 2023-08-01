using ExchangeSimulator.Application.Requests.GameRequests.CreateGame;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllPreviousGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
using ExchangeSimulator.Application.Requests.GameRequests.StartGame;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase 
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates nw game and list of starting coins
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> CreateGame(CreateGameRequest request) 
    {
        await _mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Gets all games that user can join.
    /// </summary>
    /// <returns></returns>
    [HttpGet("available-games")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllAvailableGames([FromQuery] GetAllAvailableGamesRequest request) 
    {

        var games = await _mediator.Send(request);
        return Ok(games);
    }

    /// <summary>
    /// Gets all games that user joined.
    /// </summary>
    /// <returns></returns>
    [HttpGet("current-games")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllCurrentGames([FromQuery] GetAllCurrentGamesRequest request) 
    {

        var games = await _mediator.Send(request);
        return Ok(games);
    }

    /// <summary>
    /// Gets all finished games that user joined.
    /// </summary>
    /// <returns></returns>
    [HttpGet("previous-games")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllPreviousGames([FromQuery] GetAllPreviousGamesRequest request)
    {
        var games = await _mediator.Send(request);
        return Ok(games);
    }

    /// <summary>
    /// Gets all games that user has created.
    /// </summary>
    /// <returns></returns>
    [HttpGet("owner-games")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOwnerGames([FromQuery] GetAllOwnerGamesRequest request)
    {
        var games = await _mediator.Send(request);
        return Ok(games);
    }

    /// <summary>
    /// Gets game details.
    /// </summary>
    /// <returns></returns>
    [HttpGet("{gameName}")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> GetAllOwnerGames([FromRoute] GetGameDetailsRequest request) 
    {
        var game = await _mediator.Send(request);
        return Ok(game);
    }

    /// <summary>
    /// Starts the game - changes status to active.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("start-game")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> StartGame(StartGameRequest request) {
        await _mediator.Send(request);
        return Ok();
    }
}