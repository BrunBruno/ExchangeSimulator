using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Api.Models.Order;

public class CreateOrderModel
{
    public Guid PlayerCoinId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public OrderType Type { get; set; }
}