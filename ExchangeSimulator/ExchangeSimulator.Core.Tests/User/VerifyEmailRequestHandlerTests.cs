using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.UserRequests.VerifyEmail;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class VerifyEmailRequestHandlerTests
{
    private readonly Mock<IEmailVerificationCodeRepository> _mockEmailVerificationCodeRepository;
    private readonly Mock<IPasswordHasher<EmailVerificationCode>> _mockCodeHasher;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;

    public VerifyEmailRequestHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailVerificationCodeRepository = new Mock<IEmailVerificationCodeRepository>();
        _mockCodeHasher = new Mock<IPasswordHasher<EmailVerificationCode>>();
        _mockUserContextService = new Mock<IUserContextService>();
    }

    [Fact]
    public async Task Handle_Calls_User_Repository_On_Success()
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

        var exampleCode = new EmailVerificationCode()
        {
            CodeHash = "CodeHash",
            ExpirationDate = DateTime.UtcNow.AddMinutes(15),
            Id = Guid.NewGuid(),
            UserId = exampleUser.Id
        };

        var request = new VerifyEmailRequest()
        {
            Code = "Code"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(exampleUser.Id);
        _mockEmailVerificationCodeRepository.Setup(x => x.GetCodeByUserId(exampleUser.Id)).ReturnsAsync(exampleCode);
        _mockCodeHasher.Setup(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code)).Returns(PasswordVerificationResult.Success);
        _mockUserRepository.Setup(x => x.GetUserById(exampleUser.Id)).ReturnsAsync(exampleUser);

        //when
        var handler = new VerifyEmailRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.GetCodeByUserId(exampleUser.Id), Times.Once);
        _mockCodeHasher.Verify(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(exampleUser.Id), Times.Once);
        _mockUserRepository.Verify(x => x.Update(exampleUser), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_User_Was_Not_Found()
    {
        //given
        var request = new VerifyEmailRequest()
        {
            Code = "Code"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

        //when
        var handler = new VerifyEmailRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.GetCodeByUserId(It.IsAny<Guid>()), Times.Once);
        _mockCodeHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<EmailVerificationCode>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Code_Is_Incorrect()
    {
        //given
        var exampleCode = new EmailVerificationCode()
        {
            CodeHash = "CodeHash",
            ExpirationDate = DateTime.UtcNow.AddMinutes(15),
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var request = new VerifyEmailRequest()
        {
            Code = "Code"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockEmailVerificationCodeRepository.Setup(x => x.GetCodeByUserId(It.IsAny<Guid>())).ReturnsAsync(exampleCode);
        _mockCodeHasher.Setup(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code)).Returns(PasswordVerificationResult.Failed);

        //when
        var handler = new VerifyEmailRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.GetCodeByUserId(It.IsAny<Guid>()), Times.Once);
        _mockCodeHasher.Verify(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Code_Expired()
    {
        //given
        var exampleCode = new EmailVerificationCode()
        {
            CodeHash = "CodeHash",
            ExpirationDate = DateTime.UtcNow.AddMinutes(-15),
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var request = new VerifyEmailRequest()
        {
            Code = "Code"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockEmailVerificationCodeRepository.Setup(x => x.GetCodeByUserId(It.IsAny<Guid>())).ReturnsAsync(exampleCode);
        _mockCodeHasher.Setup(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code)).Returns(PasswordVerificationResult.Success);

        //when
        var handler = new VerifyEmailRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.GetCodeByUserId(It.IsAny<Guid>()), Times.Once);
        _mockCodeHasher.Verify(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_User_Was_Not_Found()
    {
        //given
        var exampleCode = new EmailVerificationCode()
        {
            CodeHash = "CodeHash",
            ExpirationDate = DateTime.UtcNow.AddMinutes(15),
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var request = new VerifyEmailRequest()
        {
            Code = "Code"
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockEmailVerificationCodeRepository.Setup(x => x.GetCodeByUserId(It.IsAny<Guid>())).ReturnsAsync(exampleCode);
        _mockCodeHasher.Setup(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code)).Returns(PasswordVerificationResult.Success);

        //when
        var handler = new VerifyEmailRequestHandler(
            _mockEmailVerificationCodeRepository.Object,
            _mockUserContextService.Object,
            _mockUserRepository.Object,
            _mockCodeHasher.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockEmailVerificationCodeRepository.Verify(x => x.GetCodeByUserId(It.IsAny<Guid>()), Times.Once);
        _mockCodeHasher.Verify(x => x.VerifyHashedPassword(exampleCode, exampleCode.CodeHash, request.Code), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}