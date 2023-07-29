

using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Repositories;
public interface IOrderRepository {
    Task<IEnumerable<Order>> GetAllOrdersByType(OrderType type);
}

