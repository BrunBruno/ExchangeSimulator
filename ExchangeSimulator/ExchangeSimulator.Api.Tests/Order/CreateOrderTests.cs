using ExchangeSimulator.Application.Requests.GameRequests.JoinGame;
using ExchangeSimulator.Application.Requests.OrderRequests.CreateOrder;
using ExchangeSimulator.Application.Requests.PlayerRequests.GetMyPlayer;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Api.Tests.Order;

public class CreateOrderTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public CreateOrderTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task CreateOrder_Creates_Order_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var myCoin = Guid.NewGuid();
        var otherPlayerCoin = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        await _dbContext.AddPlayersAndGameForCreateOrder(gameId, "GameName", myCoin, otherPlayerCoin);

        var request = new CreateOrderRequest
        {
            GameName = "GameName",
            PlayerCoinId = myCoin,
            Price = 10,
            Quantity = 20,
            Type = OrderType.Buy
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync($"api/order", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.PlayerCoinId == myCoin);
        createdOrder.Should().BeEquivalentTo(GetExampleOrder(gameId, myCoin), opts => opts.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreateOrder_Returns_NotFound_When_Game_Was_Not_Found()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var myCoin = Guid.NewGuid();
        var otherPlayerCoin = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        await _dbContext.AddPlayersAndGameForCreateOrder(gameId, "GameName", myCoin, otherPlayerCoin);

        var request = new CreateOrderRequest
        {
            GameName = "GameName1",
            PlayerCoinId = myCoin,
            Price = 10,
            Quantity = 20,
            Type = OrderType.Buy
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync($"api/order", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_Returns_Unauthorized_When_User_Does_Not_Own_Player()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var myCoin = Guid.NewGuid();
        var otherPlayerCoin = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        await _dbContext.AddPlayersAndGameForCreateOrder(gameId, "GameName", myCoin, otherPlayerCoin);

        var request = new CreateOrderRequest
        {
            GameName = "GameName",
            PlayerCoinId = otherPlayerCoin,
            Price = 10,
            Quantity = 20,
            Type = OrderType.Buy
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync($"api/order", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_Returns_BadRequest_When_Game_Is_Not_Active()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var myCoin = Guid.NewGuid();
        var otherPlayerCoin = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        await _dbContext.AddPlayersAndGameForCreateOrder(gameId, "GameName", myCoin, otherPlayerCoin, GameStatus.Available);

        var request = new CreateOrderRequest
        {
            GameName = "GameName",
            PlayerCoinId = myCoin,
            Price = 10,
            Quantity = 20,
            Type = OrderType.Buy
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync($"api/order", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public Domain.Entities.Order GetExampleOrder(Guid gameId, Guid coinId)
        => new()
        {
            GameId = gameId,
            PlayerCoinId = coinId,
            Price = 10,
            Quantity = 20,
            Type = OrderType.Buy
        };
}