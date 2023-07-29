using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Api.Tests;

public static partial class DbFiller
{
    public static async Task AddPlayersAndGameForCreateOrder(this ExchangeSimulatorDbContext dbContext, Guid gameId, string gameName, Guid myCoin, Guid otherPlayerCoin, GameStatus status = GameStatus.Active)
    {
        var game = new Domain.Entities.Game
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = gameId,
            StartingBalance = 1000,
            Name = gameName,
            TotalPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = status,
            StartingCoins = new List<StartingCoin>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Coin1",
                    TotalBalance = 10,
                    ImageUrl = "http://image1.com"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Coin2",
                    TotalBalance = 20
                }
            },
            Players = new List<Domain.Entities.Player>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TotalBalance = 1000,
                    Name = "TestPlayerName",
                    UserId = Guid.Parse(Constants.UserId),
                    PlayerCoins = new()
                    {
                        new()
                        {
                            Id = myCoin,
                            Name = "Coin1",
                            TotalBalance = 10,
                            ImageUrl = "http://image1.com"
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Coin2",
                            TotalBalance = 20
                        }
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    TotalBalance = 1000,
                    Name = "TestPlayerName2",
                    UserId = Guid.NewGuid(),
                    PlayerCoins = new()
                    {
                        new()
                        {
                            Id = otherPlayerCoin,
                            Name = "Coin1",
                            TotalBalance = 10,
                            ImageUrl = "http://image1.com"
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Coin2",
                            TotalBalance = 20
                        }
                    }
                }
            }
        };

        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();
    }
}
