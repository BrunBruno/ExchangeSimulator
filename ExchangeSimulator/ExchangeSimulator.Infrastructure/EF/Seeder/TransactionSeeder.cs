using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Data.Seed {
    public  class TransactionSeeder {
        private static readonly Random Random = new Random();
        private readonly ExchangeSimulatorDbContext _dbContext;

        public TransactionSeeder(ExchangeSimulatorDbContext dbContext) {
            _dbContext = dbContext;
        }

        public   async Task Seed() {
 

            var gameId = Guid.Parse("f8af941c-00fc-4acc-ab9b-1ee0289ecb33");

            for (int i = 0; i < 1000; i++) {
                var transaction = new Transaction {
                    Id = Guid.NewGuid(),
                    CoinName = "BTC", // Replace with your desired coin name
                    Quantity = (decimal)Random.NextDouble() * 100, // Generate random quantity between 0 and 100
                    Price = (decimal)Random.NextDouble() * 1000, // Generate random price between 0 and 1000
                    MadeOn = DateTime.UtcNow.AddMinutes(-Random.Next(0, 60)), // Random creation date within the next hour
                    GameId = gameId,
                    RealizationId = Guid.NewGuid()
                };

                _dbContext.Transactions.Add(transaction);
            }

           await _dbContext.SaveChangesAsync();
        }
    }
}