using ExchangeSimulator.Application.Requests.RegenerateEmailVerificationCode;
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
    public async Task SignIn_Returns_Ok_With_Token_On_Success()
    {
        await _dbContext.Init();
        var userEmail = "user@gmail.com";

        await _dbContext.AddUserWithEmail(userEmail);

        var request = new SignInRequest()
        {
            Email = userEmail,
            Password = Constants.UserPassword
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        var response = await _client.PostAsync("api/user/sign-in", httpContent);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = JsonConvert.DeserializeObject<SignInDto>(await response.Content.ReadAsStringAsync());
        result.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SignIn_Returns_BadRequest_When_User_Or_Password_Is_Incorrect()
    {
        await _dbContext.Init();
        var userEmail = "user@gmail.com";

        await _dbContext.AddUserWithEmail(userEmail);

        var request = new SignInRequest()
        {
            Email = userEmail,
            Password = "Incorrect Password."
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        var response = await _client.PostAsync("api/user/sign-in", httpContent);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError); // TODO : Change to BadRequest after error handling middleware implementation.
    }
}