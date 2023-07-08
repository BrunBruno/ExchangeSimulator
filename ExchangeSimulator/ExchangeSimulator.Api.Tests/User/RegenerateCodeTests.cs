using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using ExchangeSimulator.Application.Requests.RegenerateEmailVerificationCode;

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
    public async Task RegenerateCode_Returns_Ok_On_Success()
    {
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddCodeForUser();

        var request = new RegenerateEmailVerificationCodeRequest();

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        var response = await _client.PostAsync("api/user/regenerate-code", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}