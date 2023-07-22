using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.GetAllOwnerGames;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.Game;

public class GetAllOwnerGamesRequestHandlerTests
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;

    public GetAllOwnerGamesRequestHandlerTests()
    {
        _mockGameRepository = new Mock<IGameRepository>();
        _mockUserContextService = new Mock<IUserContextService>();
    }

    [Fact]
    public async Task Handle_Should_Return_Games_On_Success()
    {
        //given
        var request = new GetAllOwnerGamesRequest()
        {
            PageNumber = 2
        };
        var games = ReturnExampleGames();
        var userId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockGameRepository.Setup(x => x.GetOwnedGamesByUserId(userId)).ReturnsAsync(games.Take(22));

        //when
        var handler = new GetAllOwnerGamesRequestHandler(_mockGameRepository.Object, _mockUserContextService.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        //then
        result.Items.Count.Should().Be(4);
        result.TotalItemsCount.Should().Be(22);
        result.ItemsFrom = 19;
        result.ItemsTo = 22;
        result.Items.First().Name.Should().Be("Game18");

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockGameRepository.Verify(x => x.GetOwnedGamesByUserId(It.IsAny<Guid>()), Times.Once);
    }

    private List<Domain.Entities.Game> ReturnExampleGames()
    {
        var gamesList = new List<Domain.Entities.Game>();

        for (var i = 0; i < 36; i++)
        {
            gamesList.Add(new Domain.Entities.Game()
            {
                CreatedAt = DateTime.UtcNow.AddDays(i),
                Description = "Description",
                Duration = TimeSpan.FromHours(20),
                Id = Guid.NewGuid(),
                Money = 1000,
                Name = $"Game{i}",
                NumberOfPlayers = 10,
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