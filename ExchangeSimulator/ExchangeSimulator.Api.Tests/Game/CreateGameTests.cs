using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using ExchangeSimulator.Application.Requests.GameRequests.CreateGame;
using ExchangeSimulator.Domain.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Api.Tests.Game;

public class CreateGameTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public CreateGameTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }


    [Fact]
    public async Task CreateGame_Should_Create_Game_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();

        var request = new CreateGameRequest
        {
            Name = "Name",
            Description = "Description",
            Password = "Password",
            StartingBalance = 1000,
            TotalPlayers = 10,
            Duration = 120,
            Coins = new List<StartingCoinItem>()
            {
                new StartingCoinItem()
                {
                    Name = "Coin1",
                    Quantity = 10
                },
                new StartingCoinItem()
                {
                    Name = "Coin2",
                    Quantity = 25
                }
            }
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync("api/game", httpContent);

        //then
        var assertDbContext = _factory.GetDbContextForAsserts();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdGame = await assertDbContext.Games
            .Include(x => x.StartingCoins)
            .FirstOrDefaultAsync();

        var createdCoins = await assertDbContext.StartingCoins
            .ToListAsync();

        var exampleGameResponse = ReturnExampleGame(request);
        var exampleCoinsResponse = ReturnExampleStartingCoinList(createdGame.Id, request);

        createdGame.Should().BeEquivalentTo(exampleGameResponse, opts =>
        {
            opts.Excluding(x => x.CreatedAt);
            opts.Excluding(x => x.PasswordHash);
            opts.Excluding(x => x.Id);
            opts.Excluding(x => x.StartingCoins);
            return opts;
        });

        createdCoins.Should().BeEquivalentTo(exampleCoinsResponse, opts =>
        {
            opts.Excluding(x => x.Id);
            opts.Excluding(x => x.Game);
            return opts;
        });
    }

    /// <summary>
    /// Number of players lower than 1.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateGame_Should_Return_BadRequest_On_Fail() 
    {
        //given
        await _dbContext.Init();

        var request = new CreateGameRequest {
            Name = "Name",
            Description = "Description",
            Password = "Password",
            StartingBalance = 1000,
            TotalPlayers = 0,
            Coins = new List<StartingCoinItem>()
            {
                new StartingCoinItem()
                {
                    Name = "Coin1",
                    Quantity = 10
                },
                new StartingCoinItem()
                {
                    Name = "Coin2",
                    Quantity = 25
                }
            }
        };


        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync("api/game", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private Domain.Entities.Game ReturnExampleGame(CreateGameRequest request)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            PasswordHash = "PasswordHash",
            StartingBalance = request.StartingBalance,
            TotalPlayers = request.TotalPlayers,
            OwnerId = Guid.Parse(Constants.UserId),
            Duration = new TimeSpan(0,120,0)
        };

    private List<StartingCoin> ReturnExampleStartingCoinList(Guid gameId, CreateGameRequest request)
        => request.Coins.Select(x => new StartingCoin
        {
            Name = x.Name,
            TotalBalance = x.Quantity,
            Id = Guid.NewGuid(),
            GameId = gameId
        }).ToList();
}