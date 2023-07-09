using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using FluentAssertions;

namespace ExchangeSimulator.Api.Tests.User;

public class RegenerateCodeTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public RegenerateCodeTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task RegenerateCode_Should_Return_Ok_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddCodeForUser();

        //when
        var response = await _client.PostAsync("api/user/regenerate-code", null);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Regenerating code, when user does not exist.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegenerateCode_Should_Returns_NotFound_On_Fail()
    {
        //given
        await _dbContext.Init();

        //when
        var response = await _client.PostAsync("api/user/regenerate-code", null);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}