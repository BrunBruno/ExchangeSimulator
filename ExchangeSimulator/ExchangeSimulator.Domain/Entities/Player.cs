namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// Player entity
/// </summary>
public class Player {

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Player name (based on UserName)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Amount of money that player has in game
    /// </summary>
    public decimal TotalBalance { get; set; }

    /// <summary>
    /// Money that are being used for orders
    /// </summary>
    public decimal LockedBalance { get; set; } = 0;

    /// <summary>
    /// Amount of order that player relaized as taker or maker
    /// </summary>
    public int TradesQuantity { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int BuyTrades { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int SellTrades { get; set; } = 0;

    /// <summary>
    /// Total volumen
    /// </summary>
    public decimal TurnOver { get; set; } = 0;

    /// <summary>
    /// Game Id
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Game to which player belongs
    /// </summary>
    public Game Game { get; set; }

    /// <summary>
    /// User id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User to which player belongs
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// List of players coins
    /// </summary>
    public List<PlayerCoin> PlayerCoins { get; set; }

    /// <summary>
    /// Amount of order that player created
    /// </summary>
    public int CreatedOrders { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int BuyCreated { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int SellCreated { get; set; } = 0;

} 