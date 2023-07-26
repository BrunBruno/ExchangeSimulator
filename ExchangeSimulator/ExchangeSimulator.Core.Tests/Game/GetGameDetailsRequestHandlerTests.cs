using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.Game;

public class GetGameDetailsRequestHandlerTests
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public GetGameDetailsRequestHandlerTests()
    {
        _mockGameRepository = new Mock<IGameRepository>();
    }

    [Fact]
    public async Task Handle_Returns_GameDetails_On_Success()
    {
        //given
        var request = new GetGameDetailsRequest
        {
            GameName = "GameName"
        };
        var game = ReturnExampleGame();

        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);

        //when
        var handler = new GetGameDetailsRequestHandler(_mockGameRepository.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        //then
        result.Should().NotBeNull();
        _mockGameRepository.Verify(x => x.GetGameByName(request.GameName), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Game_Does_Not_Exist()
    {
        //given
        var request = new GetGameDetailsRequest()
        {
            GameName = "GameName"
        };

        //when
        var handler = new GetGameDetailsRequestHandler(_mockGameRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockGameRepository.Verify(x => x.GetGameByName(request.GameName), Times.Once);
    }

    private Domain.Entities.Game ReturnExampleGame()
        => new()
        {
            CreatedAt = DateTime.UtcNow.AddDays(1),
            Description = "Description",
            Duration = TimeSpan.FromHours(20),
            Id = Guid.NewGuid(),
            Money = 1000,
            Name = "Game",
            NumberOfPlayers = 10,
            OwnerId = Guid.NewGuid(),
            PasswordHash = "PasswordHash",
            Status = GameStatus.Finished,
            StartingCoins = new(),
            Players = new()
        };
}