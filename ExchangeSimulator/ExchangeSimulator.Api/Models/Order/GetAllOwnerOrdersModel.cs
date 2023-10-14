using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Api.Models.Order;

public class GetAllOwnerOrdersModel
{
    public Guid PlayerId { get; set; }
    public int PageNumber { get; set; } = 1;
    public OrderType OrderType { get; set; } = OrderType.Buy;
}