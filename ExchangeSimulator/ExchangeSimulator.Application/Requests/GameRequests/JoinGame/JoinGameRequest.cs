using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.JoinGame;

/// <summary>
/// Request for join players to game
/// Creates player
/// Create player coin list
/// </summary>
public class JoinGameRequest : IRequest {
    public Guid GameId { get; set; }
    public string Password { get; set; }
}

