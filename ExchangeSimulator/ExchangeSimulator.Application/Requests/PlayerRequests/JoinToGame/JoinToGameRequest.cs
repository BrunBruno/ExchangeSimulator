using MediatR;

namespace ExchangeSimulator.Application.Requests.PlayerRequests.JoinToGame;

/// <summary>
/// Request for join players to game
/// Creates player
/// Create player coin list
/// Adds game to User Game list
/// Sets game to active when game is full.
/// </summary>
public class JoinToGameRequest : IRequest
{
    public string GameName { get; set; }
    public string Password { get; set; }
}

