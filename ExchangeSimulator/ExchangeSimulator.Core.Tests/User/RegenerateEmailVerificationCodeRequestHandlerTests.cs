using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.UserRequests.RegenerateEmailVerificationCode;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class RegenerateEmailVerificationCodeRequestHandlerTests
{
    private readonly Mock<IEmailVerificationCodeRepository> _mockEmailVerificationCodeRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IPasswordHasher<EmailVerificationCode>> _mockCodeHasher;
    private readonly Mock<ISmtpService> _mockSmtpService;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public RegenerateEmailVerificationCodeRequestHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserContextService = new Mock<IUserContextService>();
        _mockEmailVerificationCodeRepository = new Mock<IEmailVerificationCodeRepository>();
        _mockCodeHasher = new Mock<IPasswordHasher<EmailVerificationCode>>();
        _mockSmtpService = new Mock<ISmtpService>();
    }

    [Fact]
    public async Task Handle_Calls_Smtp_Service_On_Success()
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

        var request = new RegenerateEmailVerificationCodeRequest();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(exampleUser.Id);
        _mockUserRepository.Setup(x => x.GetUserById(exampleUser.Id)).ReturnsAsync(exampleUser);
        _mockCodeHasher.Setup(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>())).Returns(It.IsAny<string>());

        //when
        var handler = new RegenerateEmailVerificationCodeRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockCodeHasher.Object,
            _mockSmtpService.Object,
            _mockUserRepository.Object
            );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(exampleUser.Id), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.RemoveCodeByUserId(exampleUser.Id), Times.Once);
        _mockCodeHasher.Verify(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>()), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.AddCode(It.IsAny<EmailVerificationCode>()), Times.Once);
        _mockSmtpService.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_User_Was_Not_Found()
    {
        //given
        var request = new RegenerateEmailVerificationCodeRequest();

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(It.IsAny<Guid>());

        //when
        var handler = new RegenerateEmailVerificationCodeRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockCodeHasher.Object,
            _mockSmtpService.Object,
            _mockUserRepository.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.RemoveCodeByUserId(It.IsAny<Guid>()), Times.Never);
        _mockCodeHasher.Verify(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>()), Times.Never);
        _mockEmailVerificationCodeRepository.Verify(x => x.AddCode(It.IsAny<EmailVerificationCode>()), Times.Never);
        _mockSmtpService.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}