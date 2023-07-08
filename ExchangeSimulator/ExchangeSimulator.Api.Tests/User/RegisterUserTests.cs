using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ExchangeSimulator.Application.Requests.RegisterUser;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Api.Tests.User;

public class RegisterUserTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public RegisterUserTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task RegisterUser_Tests()
    {
        await _dbContext.Init();

        var request = new RegisterUserRequest
        {
            Email = "test@gmail.com",
            Username = "Test",
            Password = "Password",
            ConfirmPassword = "Password",
            ImageUrl = "http://test.com"
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        var response = await _client.PostAsync("api/user/register", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await _dbContext.Users.FirstOrDefaultAsync();
        user.Username.Should().Be("Test");
        user.Email.Should().Be("test@gmail.com");
    }
}