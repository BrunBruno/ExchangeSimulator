using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ExchangeSimulator.Application.Requests.VerifyEmail;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Api.Tests.User;

public class VerifyEmailTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public VerifyEmailTests()
    {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task VerifyEmail_Should_Set_IsVerified_To_True_On_Success()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddCodeForUser();

        var request = new VerifyEmailRequest
        {
            Code = Constants.CodeValue
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/user/verify-email", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await _dbContext.Users.FirstOrDefaultAsync();
        user.IsVerified.Should().BeTrue();
    }

    /// <summary>
    /// Verifying user without existing code.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyEmail_Should_Return_NotFound_On_Fail()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();

        var request = new VerifyEmailRequest
        {
            Code = Constants.CodeValue
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/user/verify-email", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Verifying user with incorrect code.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyEmail_Should_Return_BadRequest_On_Fail()
    {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        await _dbContext.AddCodeForUser();

        var request = new VerifyEmailRequest
        {
            Code = "Incorrect code"
        };

        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/user/verify-email", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}