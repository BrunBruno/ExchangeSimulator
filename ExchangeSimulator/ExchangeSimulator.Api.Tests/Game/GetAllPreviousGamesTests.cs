using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;

namespace ExchangeSimulator.Api.Tests.Game;

public class GetAllPreviousGamesTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public GetAllPreviousGamesTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task GetAllPreviousGames_Should_Return_Games_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddGamesWithPlayersForPagination(); //adds 20 games with players that user owns

        //when
        var response1 = await _client.GetAsync($"api/game/previous-games?gameName=&ownerName=&pageNumber=1&sortOption={GameSortOption.Date}");
        var response2 = await _client.GetAsync($"api/game/previous-games?gameName=&ownerName=&pageNumber=2&sortOption={GameSortOption.Date}");
        var response3 = await _client.GetAsync($"api/game/previous-games?gameName=&ownerName=&pageNumber=1&sortOption={GameSortOption.Name}");
        var response4 = await _client.GetAsync($"api/game/previous-games?gameName=Game5&ownerName=&pageNumber=1&sortOption={GameSortOption.Name}");

        //then
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var result1 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response1.Content.ReadAsStringAsync());
        result1.TotalItemsCount.Should().Be(6);
        result1.Items.Should().HaveCount(6);
        result1.Items.First().Name.Should().Be("Game2");

        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var result2 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response2.Content.ReadAsStringAsync());
        result2.TotalItemsCount.Should().Be(6);
        result2.Items.Should().HaveCount(0);

        response3.StatusCode.Should().Be(HttpStatusCode.OK);
        var result3 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response3.Content.ReadAsStringAsync());
        result3.TotalItemsCount.Should().Be(6);
        result3.Items.Should().HaveCount(6);
        result3.Items.First().Name.Should().Be("Game11");

        response4.StatusCode.Should().Be(HttpStatusCode.OK);
        var result4 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response4.Content.ReadAsStringAsync());
        result4.TotalItemsCount.Should().Be(1);
        result4.Items.Should().HaveCount(1);
        result4.Items.First().Name.Should().Be("Game5");
    }
}