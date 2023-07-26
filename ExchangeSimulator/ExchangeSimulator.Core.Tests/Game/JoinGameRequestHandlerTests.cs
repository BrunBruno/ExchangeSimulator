using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.GameRequests.CreateGame;
using ExchangeSimulator.Application.Requests.GameRequests.JoinGame;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExchangeSimulator.Core.Tests.Game;

public class JoinGameRequestHandlerTests
{
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly Mock<IPasswordHasher<Domain.Entities.Game>> _mockPasswordHasher;

    public JoinGameRequestHandlerTests()
    {

        _mockUserContextService = new Mock<IUserContextService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockGameRepository = new Mock<IGameRepository>();
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<Domain.Entities.Game>>();
    }

    [Fact]
    public async Task Handle_Calls_UserRepository_On_Success()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };
        var game = ReturnExampleGame(GameStatus.Available);

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.User(){Games = new()});
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);
        _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(game, game.PasswordHash, request.Password)).Returns(PasswordVerificationResult.Success);

        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object, 
            _mockUserRepository.Object, 
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_User_Was_Not_Found()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Never);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Game_Was_Not_Found()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.User());

        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Game_Is_Not_Available()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };

        var game = ReturnExampleGame(GameStatus.Active);

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.User() { Games = new() });
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);

        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Given_Password_Is_Incorrect()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };

        var game = ReturnExampleGame(GameStatus.Available);

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.User() { Games = new() });
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);
        _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(game, game.PasswordHash, request.Password)).Returns(PasswordVerificationResult.Failed);


        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_User_Has_Already_Joined_The_Game()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };

        var userId = Guid.NewGuid();

        var game = ReturnExampleGame(GameStatus.Available);

        game.Players = new()
        {
            new()
            {
                UserId = userId
            }
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.User() { Games = new() });
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);
        _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(game, game.PasswordHash, request.Password)).Returns(PasswordVerificationResult.Success);


        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_No_Available_Spots_Are_Left()
    {
        //given
        var request = new JoinGameRequest
        {
            GameName = "Game",
            Password = "Password"
        };

        var userId = Guid.NewGuid();

        var game = ReturnExampleGame(GameStatus.Available);
        game.Players = new();

        for (int i = 0; i < 10; i++)
        {
            game.Players.Add(new());
        }

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.User { Games = new() });
        _mockGameRepository.Setup(x => x.GetGameByName(request.GameName)).ReturnsAsync(game);
        _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(game, game.PasswordHash, request.Password)).Returns(PasswordVerificationResult.Success);

        //when
        var handler = new JoinGameRequestHandler
        (
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockGameRepository.Object,
            _mockPlayerRepository.Object,
            _mockPasswordHasher.Object
        );
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockGameRepository.Verify(x => x.GetGameByName(It.IsAny<string>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.Game>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.CreatePlayer(It.IsAny<Player>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    private Domain.Entities.Game ReturnExampleGame(GameStatus status)
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
        Status = status,
        StartingCoins = new(),
        Players = new()
    };
}