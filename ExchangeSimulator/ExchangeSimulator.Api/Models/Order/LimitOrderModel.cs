
namespace ExchangeSimulator.Api.Models.Order;

public class LimitOrderModel {
    public Guid PlayerCoinId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}

