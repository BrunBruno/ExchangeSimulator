using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulator.Application.Requests.RegisterUser;

/// <summary>
/// Request handler for user registration.
/// </summary>
public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ISmtpService _smtpService;
    private readonly IEmailVerificationCodeRepository _codeRepository;
    private readonly IPasswordHasher<EmailVerificationCode> _codeHasher;

    public RegisterUserRequestHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ISmtpService smtpService, IEmailVerificationCodeRepository codeRepository, IPasswordHasher<EmailVerificationCode> codeHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _smtpService = smtpService;
        _codeRepository = codeRepository;
        _codeHasher = codeHasher;
    }

    public async Task Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var emailAlreadyExists = await _userRepository.GetUserByEmail(request.Email);

        if (emailAlreadyExists is not null)
        {
            throw new BadRequestException($"User with email: {request.Email} already exists.");
        }

        if (!request.Password.Equals(request.ConfirmPassword))
        {
            throw new BadRequestException("Passwords don't match.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            ImageUrl = request.ImageUrl
        };

        var hashedPassword = _passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = hashedPassword;

        await _userRepository.AddUser(user);

        var codeValue = new Random().Next(10000, 99999).ToString();

        var code = new EmailVerificationCode() {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpirationDate = DateTime.UtcNow.AddMinutes(15),
        };

        var codeHash = _codeHasher.HashPassword(code, codeValue);

        code.CodeHash = codeHash;

        await _codeRepository.AddCode(code);

        await _smtpService.SendMessage(request.Email, "Hello " + request.Username, codeValue);
    }
}