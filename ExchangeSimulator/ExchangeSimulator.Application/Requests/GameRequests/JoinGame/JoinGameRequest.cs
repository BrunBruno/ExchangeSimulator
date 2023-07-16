using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.JoinGame;

/// <summary>
/// Request for join players to game
/// Creates player
/// Create player coin list
/// Adds game to User Game list
/// Sets game to active when game is full.
/// </summary>
public class JoinGameRequest : IRequest {
    public string GameName { get; set; }
    public string Password { get; set; }
}

