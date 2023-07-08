using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulator.Application.Requests.VerifyEmail;
public class VerifyEmailRequestHandler : IRequestHandler<VerifyEmailRequest>
{
    private readonly IEmailVerificationCodeRepository _codeRepository;
    private readonly IUserContextService _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<EmailVerificationCode> _codeHasher;

    public VerifyEmailRequestHandler(IEmailVerificationCodeRepository codeRepository, IUserContextService userContext, IUserRepository userRepository, IPasswordHasher<EmailVerificationCode> codeHasher)
    {
        _codeRepository = codeRepository;
        _userContext = userContext;
        _userRepository = userRepository;
        _codeHasher = codeHasher;
    }
    public async Task Handle(VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId()!.Value;
        var verificationCode = await _codeRepository.GetCodeByUserId(userId)
            ?? throw new NotFoundException("Code was not found.");

        var result = _codeHasher.VerifyHashedPassword(verificationCode, verificationCode.CodeHash, request.Code);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Code incorrect.");
        }

        if (verificationCode.ExpirationDate < DateTime.UtcNow)
        {
            throw new BadRequestException("Code has expired.");
        }

        var user = await _userRepository.GetUserById(userId)
            ?? throw new NotFoundException("User was not found.");

        user.IsVerified = true;
        await _userRepository.Update(user);
    }
}