using System;
using System.Net;
using System.Net.Mail;
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

    public RegisterUserRequestHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ISmtpService smtpService, IEmailVerificationCodeRepository codeRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _smtpService = smtpService;
        _codeRepository = codeRepository;
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

        var code = new EmailVerificationCode() {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = new Random().Next(10000, 99999).ToString(),
            ExpirationDate = DateTime.UtcNow.AddMinutes(15),
        };

        await _codeRepository.AddCode(code);

        await _smtpService.SendMessage(request.Email, "Hello " + request.Username, code.Code);
    }
}