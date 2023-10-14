using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;
using Microsoft.EntityFrameworkCore;
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Api.Tests.Order;

public class DeleteOrderTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public DeleteOrderTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task DeleteOrder_Deletes_Order_On_Success()
    {
        //given
        #region InitDB
        await _dbContext.Init();
        await _dbContext.AddUser();
        var coinWithOrderID = Guid.NewGuid();
        var orderToDeleteId = Guid.NewGuid();
        var game = new Domain.Entities.Game
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = Guid.NewGuid(),
            StartingBalance = 1000,
            Name = "GameName",
            TotalPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = GameStatus.Active,
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
                    LockedBalance = 40,
                    Name = "TestPlayerName",
                    UserId = Guid.Parse(Constants.UserId),
                    PlayerCoins = new()
                    {
                        new()
                        {
                            Id = coinWithOrderID,
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
            },
            Orders = new List<Domain.Entities.Order>
            {
                new()
                {
                    PlayerCoinId = coinWithOrderID,
                    Id = orderToDeleteId,
                    Price = 10,
                    Quantity = 4,
                    Type = OrderType.Buy
                }
            }
        };

        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
        #endregion

        var request = new DeleteOrderRequest
        {
            GameName = "GameName",
            OrderId = orderToDeleteId
        };

        //when
        var response = await _client.DeleteAsync($"api/game/{request.GameName}/order/{request.OrderId}");

        //then
        var assertDbContext = _factory.GetDbContextForAsserts();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var deletedOrder = await assertDbContext.Orders.FirstOrDefaultAsync();
        deletedOrder.Should().BeNull();
        var updatedPlayer = await assertDbContext.Players.FirstOrDefaultAsync();
        updatedPlayer.LockedBalance.Should().Be(0);
        updatedPlayer.TotalBalance.Should().Be(1040);
    }

    [Fact]
    public async Task DeleteOrder_Returns_NotFound_When_Order_Was_Not_Found()
    {
        //given
        #region InitDB
        await _dbContext.Init();
        await _dbContext.AddUser();
        var coinWithOrderID = Guid.NewGuid();
        var orderToDeleteId = Guid.NewGuid();
        var game = new Domain.Entities.Game
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = Guid.NewGuid(),
            StartingBalance = 1000,
            Name = "GameName",
            TotalPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = GameStatus.Active,
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
                    LockedBalance = 40,
                    Name = "TestPlayerName",
                    UserId = Guid.Parse(Constants.UserId),
                    PlayerCoins = new()
                    {
                        new()
                        {
                            Id = coinWithOrderID,
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
            },
            Orders = new List<Domain.Entities.Order>()
        };

        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
        #endregion

        var request = new DeleteOrderRequest
        {
            GameName = "GameName",
            OrderId = orderToDeleteId
        };

        //when
        var response = await _client.DeleteAsync($"api/game/{request.GameName}/order/{request.OrderId}");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOrder_Returns_Unauthorized_When_User_Does_Not_Own_The_Order()
    {
        //given
        #region InitDB
        await _dbContext.Init();
        await _dbContext.AddUser();
        var orderToDeleteId = Guid.NewGuid();
        var otherPlayerCoin = Guid.NewGuid();
        var game = new Domain.Entities.Game
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = Guid.NewGuid(),
            StartingBalance = 1000,
            Name = "GameName",
            TotalPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = GameStatus.Active,
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
                    LockedBalance = 40,
                    Name = "TestPlayerName",
                    UserId = Guid.Parse(Constants.UserId),
                    PlayerCoins = new()
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
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    TotalBalance = 1000,
                    LockedBalance = 40,
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
            },
            Orders = new List<Domain.Entities.Order>
            {
                new()
                {
                    PlayerCoinId = otherPlayerCoin,
                    Id = orderToDeleteId,
                    Price = 10,
                    Quantity = 4,
                    Type = OrderType.Buy
                }
            }
        };

        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
        #endregion

        var request = new DeleteOrderRequest
        {
            GameName = "GameName",
            OrderId = orderToDeleteId
        };

        //when
        var response = await _client.DeleteAsync($"api/game/{request.GameName}/order/{request.OrderId}");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteOrder_Returns_BadRequest_When_Game_Is_Not_Active()
    {
        //given
        #region InitDB
        await _dbContext.Init();
        await _dbContext.AddUser();
        var orderToDeleteId = Guid.NewGuid();
        var coinWithOrderId = Guid.NewGuid();
        var game = new Domain.Entities.Game
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = Guid.NewGuid(),
            StartingBalance = 1000,
            Name = "GameName",
            TotalPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = GameStatus.Finished,
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
                    LockedBalance = 40,
                    Name = "TestPlayerName",
                    UserId = Guid.Parse(Constants.UserId),
                    PlayerCoins = new()
                    {
                        new()
                        {
                            Id = coinWithOrderId,
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
            },
            Orders = new List<Domain.Entities.Order>
            {
                new()
                {
                    PlayerCoinId = coinWithOrderId,
                    Id = orderToDeleteId,
                    Price = 10,
                    Quantity = 4,
                    Type = OrderType.Buy
                }
            }
        };

        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
        #endregion

        var request = new DeleteOrderRequest
        {
            GameName = "GameName",
            OrderId = orderToDeleteId
        };

        //when
        var response = await _client.DeleteAsync($"api/game/{request.GameName}/order/{request.OrderId}");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}