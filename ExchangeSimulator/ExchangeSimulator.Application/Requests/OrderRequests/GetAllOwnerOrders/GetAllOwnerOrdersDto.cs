

using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.OrderRequests.GetAllOwnerOrders;
public class GetAllOwnerOrdersDto {
    public decimal Price { get; set; }

    public decimal Quantity { get; set; }

    public OrderType Type { get; set; }

    public string CoinName { get; set; }
    public string? CoinImageUrl { get; set; }
}
