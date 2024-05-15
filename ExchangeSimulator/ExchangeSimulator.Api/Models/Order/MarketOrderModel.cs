
namespace ExchangeSimulator.Api.Models.Order;

public class MarketOrderModel {
    public Guid PlayerCoinId { get; set; }
    public decimal Quantity { get; set; }
}
