using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.CreateGame;

/// <summary>
/// Request for game creation
/// </summary>
public class CreateGameRequest : IRequest
{
    /// <summary>
    /// game name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// game description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// password for joining the game
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// amount of starting money in game
    /// </summary>
    public decimal Money { get; set; }

    /// <summary>
    /// List of coins that will be used in game
    /// </summary>
    public List<StartingCoinItem> Coins { get; set; }

    /// <summary>
    /// Duration of the game in minutes.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Amount of players that can join the game
    /// </summary>
    public int NumberOfPlayers { get; set; }
}