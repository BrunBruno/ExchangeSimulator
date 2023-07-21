using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ExchangeSimulator.Application.Requests.UserRequests.SignIn;

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
    public async Task SignIn_Should_Return_Ok_On_Success()
    {
        //given
        await _dbContext.Init();
        var userEmail = "user@gmail.com";

        await _dbContext.AddUserWithEmail(userEmail);

        var request = new SignInRequest()
        {
            Email = userEmail,
            Password = Constants.Password
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync("api/user/sign-in", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonConvert.DeserializeObject<SignInDto>(await response.Content.ReadAsStringAsync());
        result.Token.Should().NotBeEmpty();
    }

    /// <summary>
    /// Signing in with incorrect password.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SignIn_Should_Return_BadRequest_On_Fail()
    {
        //given
        await _dbContext.Init();
        var userEmail = "user@gmail.com";

        await _dbContext.AddUserWithEmail(userEmail);

        var request = new SignInRequest()
        {
            Email = userEmail,
            Password = "IncorrectPassword"
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PostAsync("api/user/sign-in", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}