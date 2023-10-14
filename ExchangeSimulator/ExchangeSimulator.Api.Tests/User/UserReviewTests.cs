using ExchangeSimulator.Application.Requests.UserRequests.SetUserReview;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Api.Tests.User;

public class UserReviewTests : IClassFixture<TestWebApplicationFactory<Program>> {
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ExchangeSimulatorDbContext _dbContext;

    public UserReviewTests() {
        _factory = new TestWebApplicationFactory<Program>();

        _client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        var scope = scopeFactory.CreateScope();

        _dbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
    }

    [Fact]
    public async Task UserReview_Should_Return_Ok_On_Success() {
        //given
        await _dbContext.Init();
        await _dbContext.AddUser();
        var request = new SetUserReviewRequest() {
            Review = 3
        };


        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/user/user-review", httpContent);

        //then
        var assertDbContext = _factory.GetDbContextForAsserts();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await assertDbContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(Constants.UserId));
        user.Review.Should().Be(3);
    }


    /// <summary>
    /// Set review on not existing user
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UserReview_Returns_NotFound_On_Fail() {
        //given
        await _dbContext.Init();
        var request = new SetUserReviewRequest() {
            Review = 3
        };


        var json = JsonConvert.SerializeObject(request);

        var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //when
        var response = await _client.PutAsync("api/user/user-review", httpContent);

        //then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

