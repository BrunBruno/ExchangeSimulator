using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;
using ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Api.Tests.Game;

public class GetGameDetailsTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public GetGameDetailsTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task GetGameDetailsTests_Should_Return_Game_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddGameWithPlayer(Guid.NewGuid(), "GameName", GameStatus.Active);

        //when
        var response1 = await _client.GetAsync($"api/game/GameName");

        //then
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var result1 = JsonConvert.DeserializeObject<GetGameDetailsDto>(await response1.Content.ReadAsStringAsync());

        result1.Should().BeEquivalentTo(ReturnExampleResponse(), opts => opts.Excluding(x => x.CreatedAt));
    }

    [Fact]
    public async Task GetGameDetailsTests_Should_Return_NotFound_When_Game_Does_Not_Exist()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddGameWithPlayer(Guid.NewGuid(), "GameName", GameStatus.Active);

        //when
        var response1 = await _client.GetAsync($"api/game/GameName1");

        //then
        response1.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private GetGameDetailsDto ReturnExampleResponse()
        => new()
        {
            Name = "GameName",
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Money = 1000,
            CreatedAt = DateTime.UtcNow,
            Status = GameStatus.Active,
            NumberOfPlayers = 10,
            AvailableSpots = 9,
            PlayerCount = 1,
            Players = new()
            {
                new()
                {
                    Name = "TestPlayerName"
                }
            },
            Coins = new()
            {
                new()
                {
                    ImageUrl = "http://image1.com",
                    Name = "Coin1",
                    Quantity = 10
                },
                new()
                {
                    Name = "Coin2",
                    Quantity = 20
                }
            }
        };
}