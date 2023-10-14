using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllCurrentGames;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.Game;

public class GetAllCurrentGamesRequestHandlerTests
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;

    public GetAllCurrentGamesRequestHandlerTests()
    {
        _mockGameRepository = new Mock<IGameRepository>();
        _mockUserContextService = new Mock<IUserContextService>();
    }

    [Fact]
    public async Task Handle_Should_Return_Games_On_Success()
    {
        //given
        var request = new GetAllCurrentGamesRequest();
        var games = ReturnExampleGames();
        var userId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockGameRepository.Setup(x => x.GetGamesByUserId(userId)).ReturnsAsync(games.Take(10));

        //when
        var handler = new GetAllCurrentGamesRequestHandler(_mockGameRepository.Object, _mockUserContextService.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        //then
        result.Items.Count.Should().Be(6);
        result.TotalItemsCount.Should().Be(7);
        result.ItemsFrom = 1;
        result.ItemsTo = 6;
        result.Items.First().Name.Should().Be("Game9");

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockGameRepository.Verify(x => x.GetGamesByUserId(It.IsAny<Guid>()), Times.Once);
    }

    private List<Domain.Entities.Game> ReturnExampleGames()
    {
        var gamesList = new List<Domain.Entities.Game>();

        for (var i = 0; i < 12; i++)
        {
            gamesList.Add(new Domain.Entities.Game()
            {
                CreatedAt = DateTime.UtcNow.AddDays(i),
                Description = "Description",
                Duration = TimeSpan.FromHours(20),
                Id = Guid.NewGuid(),
                StartingBalance = 1000,
                Name = $"Game{i}",
                TotalPlayers = 10,
                OwnerId = Guid.NewGuid(),
                PasswordHash = "PasswordHash",
                Status = (GameStatus)(i % 3),
                Players = new List<Domain.Entities.Player>(),
                Owner = new Domain.Entities.User()
            });
        }

        return gamesList;
    }
}