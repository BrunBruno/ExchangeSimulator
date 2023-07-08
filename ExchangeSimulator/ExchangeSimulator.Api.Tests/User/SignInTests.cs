using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ExchangeSimulator.Application.Requests.SignIn;

namespace ExchangeSimulator.Api.Tests.User;

public class SignInTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public SignInTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task SignIn_Tests()
    {
        await _dbContext.Init();
        var userEmail = "user@gmail.com";

        await _dbContext.AddUserWithEmail(userEmail);

        var request1 = new SignInRequest()
        {
            Email = userEmail,
            Password = Constants.UserPassword
        };

        var request2 = new SignInRequest()
        {
            Email = userEmail,
            Password = "IncorrectPassword"
        };

        var json1 = JsonConvert.SerializeObject(request1);
        var json2 = JsonConvert.SerializeObject(request2);

        var httpContent1 = new StringContent(json1, UnicodeEncoding.UTF8, "application/json");
        var httpContent2 = new StringContent(json2, UnicodeEncoding.UTF8, "application/json");

        var response1 = await _client.PostAsync("api/user/sign-in", httpContent1);
        var response2 = await _client.PostAsync("api/user/sign-in", httpContent2);

        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.InternalServerError); // TODO : Change to BadRequest after error handling middleware implementation.

        var result = JsonConvert.DeserializeObject<SignInDto>(await response1.Content.ReadAsStringAsync());
        result.Token.Should().NotBeEmpty();

        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}