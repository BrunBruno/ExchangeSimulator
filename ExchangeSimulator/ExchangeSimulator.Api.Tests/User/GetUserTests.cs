using ExchangeSimulator.Application.Requests.UserRequests.GetUser;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace ExchangeSimulator.Api.Tests.User;
public class GetUserTests : IClassFixture<TestWebApplicationFactory<Program>> {
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public GetUserTests() {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task GetUser_Should_Return_Ok_On_Success() {
        await _dbContext.Init();

        await _dbContext.AddUser();

        //when
        var response = await _client.GetAsync("api/user/");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
        result.Email.Should().Be("test@gmail.com");
        result.UserName.Should().NotBeEmpty();


    }


    /// <summary>
    /// Get not existing user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetUser_Returns_NotFound_On_Fail() {
        await _dbContext.Init();

        //when
        var response = await _client.GetAsync("api/user/");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

