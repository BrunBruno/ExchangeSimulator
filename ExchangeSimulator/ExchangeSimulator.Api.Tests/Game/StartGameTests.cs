using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ExchangeSimulator.Application.Requests.GameRequests.StartGame;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Api.Tests.Game;

public class StartGameTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public StartGameTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task StartGame_Should_Change_Game_Status_To_Active()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var gameId = Guid.NewGuid();
        await _dbContext.AddGame(gameId, "Game1", GameStatus.Available);

        var request = new StartGameRequest
        {
            GameName = "Game1"
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/game/start-game", httpContent);

        //then
        var assertDbContext = _factory.GetDbContextForAsserts();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var startedGame = await assertDbContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
        startedGame.Status.Should().Be(GameStatus.Active);
    }

    /// <summary>
    /// Start game that not exists
    /// </summary>
    /// <returns></returns>

    [Fact]
    public async Task StartGame_Returns_NotFound_On_Fail() {
        await _dbContext.Init();
        await _dbContext.AddUser();

        var request = new StartGameRequest {
            GameName = "Game-That-Not-Exists"
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/game/start-game", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Start game width active status (only available should be startable)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task StartGame_Returns_BadRequest_On_Fail() {
        await _dbContext.Init();
        await _dbContext.AddUser();
        var gameId = Guid.NewGuid();
        await _dbContext.AddGame(gameId, "Game1", GameStatus.Active);


        var request = new StartGameRequest {
            GameName = "Game1"
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/game/start-game", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}