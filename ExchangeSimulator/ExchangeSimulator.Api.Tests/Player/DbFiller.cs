using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Api.Tests;

public static partial class DbFiller
{
    public static async Task AddPlayerAndGame(this ExchangeSimulatorDbContext dbContext, string gameName, Guid coinId1, Guid coinId2)
    {
        var game = new Domain.Entities.Game
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = Guid.NewGuid(),
            Money = 1000,
            Name = gameName,
            NumberOfPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = GameStatus.Active,
            StartingCoins = new List<StartingCoin>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Coin1",
                    Quantity = 10,
                    ImageUrl = "http://image1.com"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Coin2",
                    Quantity = 20
                }
            },
            Players = new List<Domain.Entities.Player>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Money = 1000,
                    Name = "TestPlayerName",
                    UserId = Guid.Parse(Constants.UserId),
                    PlayerCoins = new()
                    {
                        new()
                        {
                            Id = coinId1,
                            Name = "Coin1",
                            Quantity = 10,
                            ImageUrl = "http://image1.com"
                        },
                        new()
                        {
                            Id = coinId2,
                            Name = "Coin2",
                            Quantity = 20
                        }
                    }
                }
            }
        };

        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();
    }
}