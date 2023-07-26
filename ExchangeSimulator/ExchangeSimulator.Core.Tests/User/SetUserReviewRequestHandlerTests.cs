using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.UserRequests.SetUserReview;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class SetUserReviewRequestHandlerTests
{
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public SetUserReviewRequestHandlerTests()
    {
        _mockUserContextService = new Mock<IUserContextService>();
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task Handle_Calls_UserRepository_On_Success()
    {
        //given
        var request = new SetUserReviewRequest()
        {
            Review = 3
        };
        var userId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync(new Domain.Entities.User());

        //when
        var handler = new SetUserReviewRequestHandler(_mockUserContextService.Object, _mockUserRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_User_Does_Not_Exist()
    {
        //given
        var request = new SetUserReviewRequest()
        {
            Review = 3
        };
        var userId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);

        //when
        var handler = new SetUserReviewRequestHandler(_mockUserContextService.Object, _mockUserRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}