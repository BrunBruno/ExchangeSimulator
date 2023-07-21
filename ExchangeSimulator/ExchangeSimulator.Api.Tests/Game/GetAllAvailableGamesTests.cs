using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Pagination.Enums;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllAvailableGames;

namespace ExchangeSimulator.Api.Tests.Game;

public class GetAllAvailableGamesTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public GetAllAvailableGamesTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task GetAllAvailableGames_Should_Return_Games_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddGamesForPagination(); //adds 20 games

        //when
        var response1 = await _client.GetAsync($"api/game/available-games?gameName=&ownerName=&pageNumber=1&sortOption={GameSortOption.Date}");
        var response2 = await _client.GetAsync($"api/game/available-games?gameName=&ownerName=&pageNumber=2&sortOption={GameSortOption.Date}");
        var response3 = await _client.GetAsync($"api/game/available-games?gameName=&ownerName=&pageNumber=2&sortOption={GameSortOption.Name}");
        var response4 = await _client.GetAsync($"api/game/available-games?gameName=Game12&ownerName=&pageNumber=1&sortOption={GameSortOption.Name}");

        //then
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var result1 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response1.Content.ReadAsStringAsync());
        result1.TotalItemsCount.Should().Be(7);
        result1.Items.Should().HaveCount(6);
        result1.Items.First().Name.Should().Be("Game0");

        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var result2 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response2.Content.ReadAsStringAsync());
        result2.TotalItemsCount.Should().Be(7);
        result2.Items.Should().HaveCount(1);
        result2.Items.First().Name.Should().Be("Game18");

        response3.StatusCode.Should().Be(HttpStatusCode.OK);
        var result3 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response3.Content.ReadAsStringAsync());
        result3.TotalItemsCount.Should().Be(7);
        result3.Items.Should().HaveCount(1);
        result3.Items.First().Name.Should().Be("Game9");

        response4.StatusCode.Should().Be(HttpStatusCode.OK);
        var result4 = JsonConvert.DeserializeObject<PagedResult<GetAllAvailableGamesDto>>(await response4.Content.ReadAsStringAsync());
        result4.TotalItemsCount.Should().Be(1);
        result4.Items.Should().HaveCount(1);
        result4.Items.First().Name.Should().Be("Game12");
    }
}