using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.UserRequests.IsEmailVerified;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class IsEmailVerifiedRequestHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;

    public IsEmailVerifiedRequestHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserContextService = new Mock<IUserContextService>();
    }

    [Fact]
    public async Task Handle_Returns_IsVerified_On_Success()
    {
        //given
        var exampleUser = new Domain.Entities.User
        {
            Email = "test@test.com",
            Username = "Username",
            Id = Guid.NewGuid(),
            ImageUrl = "http://test.com",
            IsVerified = true,
            PasswordHash = "PasswordHash",
            RoleId = (int)Roles.User
        };

        var request = new IsEmailVerifiedRequest();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(exampleUser.Id);
        _mockUserRepository.Setup(x => x.GetUserById(exampleUser.Id)).ReturnsAsync(exampleUser);

        //when
        var handler = new IsEmailVerifiedRequestHandler(_mockUserContextService.Object, _mockUserRepository.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        //then
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(exampleUser.Id), Times.Once);
        result.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_User_Was_Not_Found()
    {
        //given
        var request = new IsEmailVerifiedRequest();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

        //when
        var handler = new IsEmailVerifiedRequestHandler(_mockUserContextService.Object, _mockUserRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
    }
}