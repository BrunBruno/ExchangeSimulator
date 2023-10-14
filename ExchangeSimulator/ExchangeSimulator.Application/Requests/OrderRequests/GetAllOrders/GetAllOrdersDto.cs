using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOrders;

public class GetAllOrdersDto 
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public OrderType OrderType { get; set; }
    public string CoinName { get; set; }
    public string? CoinImage { get; set; }
}