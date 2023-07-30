using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Api.Models.Order;

public class GetAllOrdersModel
{
    public OrderType OrderType { get; set; }
    public int ElementsCount { get; set; } = 10;
    public string? CoinName { get; set; }
}