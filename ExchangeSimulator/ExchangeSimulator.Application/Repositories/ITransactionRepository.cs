
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;
public interface ITransactionRepository {
    Task CreateTransaction(Transaction transaction);
}

