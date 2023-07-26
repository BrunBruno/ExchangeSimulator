using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.UserRequests.GetUser;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class GetUserRequestHandlerTests
{
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public GetUserRequestHandlerTests()
    {
        _mockUserContextService = new Mock<IUserContextService>();
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task Handle_Returns_User_On_Success()
    {
        //given
        var request = new GetUserRequest();
        var userId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync(new Domain.Entities.User());

        //when
        var handler = new GetUserRequestHandler(_mockUserContextService.Object, _mockUserRepository.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        //then
        result.Should().NotBeNull();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_User_Does_Not_Exist()
    {
        //given
        var request = new GetUserRequest();
        var userId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);

        //when
        var handler = new GetUserRequestHandler(_mockUserContextService.Object, _mockUserRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
    }
}