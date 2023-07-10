using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ExchangeSimulator.Application.Services;

namespace ExchangeSimulator.Application.Requests.UserRequests.SignIn;

/// <summary>
/// Request handler for signing in.
/// </summary>
public class SignInRequestHandler : IRequestHandler<SignInRequest, SignInDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtService _jwtService;

    public SignInRequestHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<SignInDto> Handle(SignInRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(request.Email);

        if (user is null)
        {
            throw new BadRequestException("Invalid email or password.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid email or password");
        }

        var token = _jwtService.GetJwtToken(user);

        return new SignInDto()
        {
            Token = token
        };
    }
}