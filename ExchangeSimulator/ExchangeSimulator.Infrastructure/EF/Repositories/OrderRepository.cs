using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

public class OrderRepository : IOrderRepository 
{
    private readonly ExchangeSimulatorDbContext _dbContext;

    public OrderRepository(ExchangeSimulatorDbContext dbContext) 
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetOrderById(Guid orderId)
        => await _dbContext.Orders
            .Include(x => x.Game)
            .Include(x => x.PlayerCoin)
            .ThenInclude(x => x.Player)
            .FirstOrDefaultAsync(x => x.Id == orderId);

    public async Task Update(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }
}