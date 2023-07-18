using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;

public class GetAllOwnerGamesDto
{
    /// <summary>
    /// Game name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when game ends
    /// </summary>
    public DateTime EndGame { get; set; }

    /// <summary>
    /// Number of players that joined the game
    /// </summary>
    public int PlayerCount { get; set; }

    /// <summary>
    /// Max players spots - players that joined
    /// </summary>
    public int AvailableSpots { get; set; }

    /// <summary>
    /// Starting money
    /// </summary>
    public decimal Money { get; set; }

    /// <summary>
    /// Owner of the game
    /// </summary>
    public string OwnerName { get; set; }

    /// <summary>
    /// Game status
    /// </summary>
    public GameStatus Status { get; set; }

    /// <summary>
    /// List of players
    /// </summary>
    public List<PlayerDto> Players { get; set; }

    /// <summary>
    /// List of starting coins
    /// </summary>
    public List<CoinDto> Coins { get; set; }
}