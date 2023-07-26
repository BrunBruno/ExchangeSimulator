using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSimulator.Infrastructure.Seeders {
    public class GameSeeder {
        private readonly ExchangeSimulatorDbContext dbContext;
        private static Random random = new Random();

        public GameSeeder(ExchangeSimulatorDbContext dbContext) {
            this.dbContext = dbContext;
        }
        public async Task Seed() { 
            var games = new List<Game>();
            for (int i = 0; i < 100; i++) {
                var game = new Game {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(16),
                    Description = GenerateRandomString(256),
                    PasswordHash = "",
                    Money = (decimal)random.Next(1000, 1000000),
                    Duration = TimeSpan.FromDays(random.Next(7, 30)),
                    NumberOfPlayers = random.Next(2, 1000),
                    OwnerId = Guid.Parse("df6434f4-5fcc-4f8b-ada4-682a1c3a553f"),
                };

                game.StartingCoins = GenerateRandomCoins(game.Id);

                games.Add(game);
            }

            await dbContext.Games.AddRangeAsync(games);
            await dbContext.SaveChangesAsync();

        }

        public static string GenerateRandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++) {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            return sb.ToString();
        }

        private List<StartingCoin> GenerateRandomCoins(Guid gameId) {
            var coins = new List<StartingCoin>();
            const int maxCoins = 5;

            for (int i = 0; i < maxCoins; i++) {
                var coin = new StartingCoin {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(8),
                    Quantity = random.Next(1, 100),
                    ImageUrl = null,
                    GameId = gameId
                };

                coins.Add(coin);
            }

            return coins;
        }
    }
}
