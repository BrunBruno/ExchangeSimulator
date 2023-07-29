

using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;
public class OrderRepository : IOrderRepository {
    private readonly ExchangeSimulatorDbContext _dbContext;

    public OrderRepository(ExchangeSimulatorDbContext dbContext) {
        _dbContext = dbContext;
    }
}

