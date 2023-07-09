using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.RegisterUser;
using ExchangeSimulator.Application.Requests.SignIn;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExchangeSimulator.Core.Tests.User;

public class SignInRequestHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher<Domain.Entities.User>> _mockPasswordHasher;
    private readonly Mock<IJwtService> _mockJwtService;

    public SignInRequestHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<Domain.Entities.User>>();
        _mockJwtService = new Mock<IJwtService>();
    }

    [Fact]
    public async Task Handle_Returns_Token_On_Success()
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

        var request = new SignInRequest()
        {
            Email = "test@test.com",
            Password = "Password@123456"
        };

        _mockUserRepository.Setup(x => x.GetUserByEmail(request.Email)).ReturnsAsync(exampleUser);
        _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(exampleUser,exampleUser.PasswordHash,request.Password)).Returns(PasswordVerificationResult.Success);
        _mockJwtService.Setup(x => x.GetJwtToken(exampleUser)).Returns("token");

        //when
        var handler = new SignInRequestHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtService.Object
        );

        var result = await handler.Handle(request, CancellationToken.None);

        //then
        result.Should().BeEquivalentTo(new SignInDto()
        {
            Token = "token"
        });
        _mockUserRepository.Verify(x => x.GetUserByEmail(request.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(exampleUser, exampleUser.PasswordHash, request.Password), Times.Once);
        _mockJwtService.Verify(x => x.GetJwtToken(exampleUser), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Email_Is_Invalid()
    {
        //given
        var request = new SignInRequest()
        {
            Email = "test@test.com",
            Password = "Password@123456"
        };

        //when
        var handler = new SignInRequestHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtService.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockUserRepository.Verify(x => x.GetUserByEmail(request.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(It.IsAny<Domain.Entities.User>(), It.IsAny<string>(), request.Password), Times.Never);
        _mockJwtService.Verify(x => x.GetJwtToken(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Password_Is_Invalid()
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

        var request = new SignInRequest()
        {
            Email = "test@test.com",
            Password = "Password@123456"
        };

        _mockUserRepository.Setup(x => x.GetUserByEmail(request.Email)).ReturnsAsync(exampleUser);
        _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(exampleUser, exampleUser.PasswordHash, request.Password)).Returns(PasswordVerificationResult.Failed);

        //when
        var handler = new SignInRequestHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtService.Object
        );

        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();
        _mockUserRepository.Verify(x => x.GetUserByEmail(request.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyHashedPassword(exampleUser, exampleUser.PasswordHash, request.Password), Times.Once);
        _mockJwtService.Verify(x => x.GetJwtToken(exampleUser), Times.Never);
    }
}