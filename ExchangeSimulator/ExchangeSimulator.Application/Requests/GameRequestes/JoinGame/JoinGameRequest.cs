using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequestes.JoinGame;

/// <summary>
/// Request for join players to game
/// Creates player
/// Create player coin list
/// </summary>
public class JoinGameRequest : IRequest {
    public string GameName { get; set; }
}

