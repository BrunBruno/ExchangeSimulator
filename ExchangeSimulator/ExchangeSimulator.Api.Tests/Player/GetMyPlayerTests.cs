using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using ExchangeSimulator.Application.Requests.PlayerRequests.GetMyPlayer;

namespace ExchangeSimulator.Api.Tests.Player;

public class GetMyPlayerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public GetMyPlayerTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }


    [Fact]
    public async Task GetMyPlayer_Should_Return_Player_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var coinId1 = Guid.NewGuid();
        var coinId2 = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        await _dbContext.AddPlayerAndGame("GameName", coinId1, coinId2, playerId);

        var request = new GetMyPlayerRequest
        {
            GameName = "GameName"
        };

        //when
        var response = await _client.GetAsync($"api/player/my?gameName={request.GameName}");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result1 = JsonConvert.DeserializeObject<GetMyPlayerDto>(await response.Content.ReadAsStringAsync());
        result1.Should().BeEquivalentTo(GetExampleResponse(coinId1, coinId2, playerId));
    }

    [Fact]
    public async Task GetMyPlayer_Should_Return_NotFound_When_Game_For_User_Was_Not_Found()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var coinId1 = Guid.NewGuid();
        var coinId2 = Guid.NewGuid();
        await _dbContext.AddPlayerAndGame("GameName", coinId1, coinId2, Guid.NewGuid());

        var request = new GetMyPlayerRequest
        {
            GameName = "GameName1"
        };

        //when
        var response = await _client.GetAsync($"api/player/my?gameName={request.GameName}");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public GetMyPlayerDto GetExampleResponse(Guid coinId1, Guid coinId2, Guid playerId)
        => new()
        {
            Id = playerId,
            TotalBalance = 1000,
            Name = "TestPlayerName",
            TradesQuantity = 0,
            TurnOver = 0,
            PlayerCoins = new()
            {
                new()
                {
                    Id = coinId1,
                    Name = "Coin1",
                    TotalBalance = 10,
                    ImageUrl = "http://image1.com"
                },
                new()
                {
                    Id = coinId2,
                    Name = "Coin2",
                    TotalBalance = 20
                }
            }
        };
}