using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.CreateGame;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExchangeSimulator.Core.Tests.Game;

public class CreateGameRequestHandlerTests
{
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IPasswordHasher<Domain.Entities.Game>> _mockPasswordHasher;

    public CreateGameRequestHandlerTests()
    {
        _mockUserContextService = new Mock<IUserContextService>();
        _mockGameRepository = new Mock<IGameRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<Domain.Entities.Game>>();
    }

    [Fact]
    public async Task Handle_Calls_GameRepository_On_Success()
    {
        //given
        var request = new CreateGameRequest
        {
            Coins = new List<StartingCoinItem>(),
            Description = "Description",
            Money = 1000,
            Name = "Name",
            NumberOfPlayers = 1,
            Password = "Password",
            Duration = 120
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockGameRepository.Setup(x => x.GetGameByName(request.Name)).ReturnsAsync(default(Domain.Entities.Game));
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<Domain.Entities.Game>(), request.Password)).Returns("PasswordHash");

        //when
        var handler = new CreateGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object, _mockPasswordHasher.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(request.Name), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<Domain.Entities.Game>(), request.Password), Times.Once);
        _mockGameRepository.Verify(x => x.CreateGame(It.IsAny<Domain.Entities.Game>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_NumberOfPlayers_Is_Lower_Than_One()
    {
        //given
        var request = new CreateGameRequest
        {
            Coins = new List<StartingCoinItem>(),
            Description = "Description",
            Money = 1000,
            Name = "Name",
            NumberOfPlayers = 0,
            Password = "Password",
            Duration = 120
        };

        //when
        var handler = new CreateGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object, _mockPasswordHasher.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Never);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Never);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>()), Times.Never);
        _mockGameRepository.Verify(x => x.CreateGame(It.IsAny<Domain.Entities.Game>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_GameName_Already_Exists()
    {
        //given
        var request = new CreateGameRequest
        {
            Coins = new List<StartingCoinItem>(),
            Description = "Description",
            Money = 1000,
            Name = "Name",
            NumberOfPlayers = 1,
            Password = "Password",
            Duration = 120
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockGameRepository.Setup(x => x.GetGameByName(request.Name)).ReturnsAsync(new Domain.Entities.Game());

        //when
        var handler = new CreateGameRequestHandler(_mockUserContextService.Object, _mockGameRepository.Object, _mockPasswordHasher.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(request.Name), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<Domain.Entities.Game>(), request.Password), Times.Never);
        _mockGameRepository.Verify(x => x.CreateGame(It.IsAny<Domain.Entities.Game>()), Times.Never);
    }
}