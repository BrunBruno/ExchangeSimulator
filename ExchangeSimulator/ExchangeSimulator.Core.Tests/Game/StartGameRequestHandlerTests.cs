using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.StartGame;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.Game;

public class StartGameRequestHandlerTests
{
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IGameRepository> _mockGameRepository;

    public StartGameRequestHandlerTests()
    {
        _mockUserContextService = new Mock<IUserContextService>();
        _mockGameRepository = new Mock<IGameRepository>();
    }

    [Fact]
    public async Task Handle_Calls_GameRepository_On_Success()
    {
        //given
        var request = new StartGameRequest()
        {
            GameName = "GameName"
        };
        var userId = Guid.NewGuid();

        var game = new Domain.Entities.Game
        {
            OwnerId = userId,
            Status = GameStatus.Available
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);

        //when
        var handler = new StartGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();
        _mockGameRepository.Verify(x => x.GetGameByName(request.GameName), Times.Once);
        _mockGameRepository.Verify(x => x.Update(game), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Game_Does_Not_Exist()
    {
        //given
        var request = new StartGameRequest()
        {
            GameName = "GameName"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

        //when
        var handler = new StartGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockGameRepository.Verify(x => x.GetGameByName(request.GameName), Times.Once);
        _mockGameRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Game>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_User_Is_Not_The_Owner_Of_The_Game()
    {
        //given
        var request = new StartGameRequest()
        {
            GameName = "GameName"
        };

        var game = new Domain.Entities.Game
        {
            OwnerId = Guid.NewGuid(),
            Status = GameStatus.Available
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);

        //when
        var handler = new StartGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockGameRepository.Verify(x => x.GetGameByName(request.GameName), Times.Once);
        _mockGameRepository.Verify(x => x.Update(game), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Game_Is_Not_Available()
    {
        //given
        var request = new StartGameRequest()
        {
            GameName = "GameName"
        };
        var userId = Guid.NewGuid();

        var game = new Domain.Entities.Game
        {
            OwnerId = userId,
            Status = GameStatus.Active
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);

        //when
        var handler = new StartGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockGameRepository.Verify(x => x.GetGameByName(request.GameName), Times.Once);
        _mockGameRepository.Verify(x => x.Update(game), Times.Never);
    }
}