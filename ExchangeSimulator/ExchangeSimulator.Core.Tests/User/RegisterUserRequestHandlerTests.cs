using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.UserRequests.RegisterUser;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class RegisterUserRequestHandlerTests
{
    private readonly Mock<IEmailVerificationCodeRepository> _mockEmailVerificationCodeRepository;
    private readonly Mock<IPasswordHasher<EmailVerificationCode>> _mockCodeHasher;
    private readonly Mock<IPasswordHasher<Domain.Entities.User>> _mockPasswordHasher;
    private readonly Mock<ISmtpService> _mockSmtpService;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public RegisterUserRequestHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailVerificationCodeRepository = new Mock<IEmailVerificationCodeRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<Domain.Entities.User>>();
        _mockCodeHasher = new Mock<IPasswordHasher<EmailVerificationCode>>();
        _mockSmtpService = new Mock<ISmtpService>();
    }

    [Fact]
    public async Task Handle_Calls_Smtp_Service_On_Success()
    {
        //given
        var request = new RegisterUserRequest()
        {
            Email = "test@test.com",
            Password = "Password@123456",
            ConfirmPassword = "Password@123456",
            ImageUrl = "http://test.com",
            Username = "Username"
        };

        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<Domain.Entities.User>(), It.IsAny<string>())).Returns(It.IsAny<string>());
        _mockCodeHasher.Setup(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>())).Returns(It.IsAny<string>());

        //when
        var handler = new RegisterUserRequestHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockSmtpService.Object,
            _mockEmailVerificationCodeRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();
        _mockUserRepository.Verify(x => x.GetUserByEmail(request.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<Domain.Entities.User>(), request.Password), Times.Once);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<Domain.Entities.User>()), Times.Once);
        _mockCodeHasher.Verify(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>()), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.AddCode(It.IsAny<EmailVerificationCode>()), Times.Once);
        _mockSmtpService.Verify(x => x.SendMessage(request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_BadRequest_Exception_When_Email_Is_Taken()
    {
        //given
        var request = new RegisterUserRequest()
        {
            Email = "test@test.com",
            Password = "Password@123456",
            ConfirmPassword = "Password@123456",
            ImageUrl = "http://test.com",
            Username = "Username"
        };

        _mockUserRepository.Setup(x => x.GetUserByEmail(request.Email)).ReturnsAsync(new Domain.Entities.User());

        //when
        var handler = new RegisterUserRequestHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockSmtpService.Object,
            _mockEmailVerificationCodeRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockUserRepository.Verify(x => x.GetUserByEmail(request.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<Domain.Entities.User>(), request.Password), Times.Never);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockCodeHasher.Verify(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>()), Times.Never);
        _mockEmailVerificationCodeRepository.Verify(x => x.AddCode(It.IsAny<EmailVerificationCode>()), Times.Never);
        _mockSmtpService.Verify(x => x.SendMessage(request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Passwords_Do_Not_Match()
    {
        //given
        var request = new RegisterUserRequest()
        {
            Email = "test@test.com",
            Password = "Password@123456",
            ConfirmPassword = "Password",
            ImageUrl = "http://test.com",
            Username = "Username"
        };

        //when
        var handler = new RegisterUserRequestHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockSmtpService.Object,
            _mockEmailVerificationCodeRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockUserRepository.Verify(x => x.GetUserByEmail(request.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<Domain.Entities.User>(), request.Password), Times.Never);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockCodeHasher.Verify(x => x.HashPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>()), Times.Never);
        _mockEmailVerificationCodeRepository.Verify(x => x.AddCode(It.IsAny<EmailVerificationCode>()), Times.Never);
        _mockSmtpService.Verify(x => x.SendMessage(request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}