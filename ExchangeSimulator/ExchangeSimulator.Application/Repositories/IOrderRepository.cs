using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

public interface IOrderRepository 
{
    Task<Order?> GetOrderById(Guid orderId);
    Task Update(Order order);
}