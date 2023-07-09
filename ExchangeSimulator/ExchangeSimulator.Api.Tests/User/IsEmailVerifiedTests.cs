using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using ExchangeSimulator.Application.Requests.IsEmailVerified;
using FluentAssertions;

namespace ExchangeSimulator.Api.Tests.User;

public class IsEmailVerifiedTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public IsEmailVerifiedTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task IsEmailVerified_Should_Return_IsVerified_Property_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();

        //when
        var response = await _client.GetAsync("api/user/is-verified");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonConvert.DeserializeObject<IsEmailVerifiedDto>(await response.Content.ReadAsStringAsync());
        result.IsEmailVerified.Should().Be(false);
    }

    /// <summary>
    /// Checking if email is verified for not existing user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IsEmailVerified_Should_Return_NotFound_On_Fail()
    {
        //given
        await _dbContext.Init();

        //when
        var response = await _client.GetAsync("api/user/is-verified");

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
