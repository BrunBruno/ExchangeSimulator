
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

internal class TransactionRepository : ITransactionRepository {
    private readonly ExchangeSimulatorDbContext _dbContext;
    public TransactionRepository(ExchangeSimulatorDbContext dbContext) {
        _dbContext = dbContext;
    }
    public async Task CreateTransaction(Transaction transaction) {
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();    
    }
}

