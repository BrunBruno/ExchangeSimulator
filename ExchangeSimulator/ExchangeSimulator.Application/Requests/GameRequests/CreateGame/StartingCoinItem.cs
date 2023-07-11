namespace ExchangeSimulator.Application.Requests.GameRequests.CreateGame;

/// <summary>
/// Item that represents coin
/// </summary>
public class StartingCoinItem
{
    /// <summary>
    /// Coin name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Amount of coins
    /// </summary>
    public decimal Quantity { get; set; }
}