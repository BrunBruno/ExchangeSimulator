using ExchangeSimulator.Api.Hubs;
using ExchangeSimulator.Application.Hubs;
using ExchangeSimulator.Application.Requests.GameRequests.CreateGame;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllPreviousGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
using ExchangeSimulator.Application.Requests.GameRequests.JoinGame;
using ExchangeSimulator.Application.Requests.GameRequests.StartGame;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulator.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase 
{
    private readonly IMediator _mediator;
    private readonly IHubContext<GameHub, IGameHub> _hub;

    public GameController(IMediator mediator, IHubContext<GameHub,IGameHub> hub) 
    {
        _mediator = mediator;
        _hub = hub;
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
    /// Joins player to game
    /// Creates new player
    /// Create Player coins list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("join-game")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> JoinGame(JoinGameRequest request) 
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
    /// Starts the game - changes status to active.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("start-game")]
    [Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> StartGame(StartGameRequest request) 
    { 
        await _mediator.Send(request);
        return Ok();
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
    /// Enters the game (connects to hub).
    /// </summary>
    /// <returns></returns>
    [HttpGet("order/{gameName}")]
    //[Authorize(Policy = "IsVerified")]
    public async Task<IActionResult> OrdersChanged([FromRoute] string gameName)
    {
        await _hub.Clients.Groups(gameName).OrdersChanged(gameName);
        return Ok();
    }
}