using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulator.Application.Requests.UserRequests.RegenerateEmailVerificationCode;

/// <summary>
/// Handler for regenerating email verification code.
/// </summary>
public class RegenerateEmailVerificationCodeRequestHandler : IRequestHandler<RegenerateEmailVerificationCodeRequest>
{
    private readonly IEmailVerificationCodeRepository _emailVerificationCodeRepository;
    private readonly IUserContextService _userContextService;
    private readonly IPasswordHasher<EmailVerificationCode> _codeHasher;
    private readonly ISmtpService _smtpService;
    private readonly IUserRepository _userRepository;

    public RegenerateEmailVerificationCodeRequestHandler(IEmailVerificationCodeRepository emailVerificationCodeRepository, IUserContextService userContextService, IPasswordHasher<EmailVerificationCode> codeHasher, ISmtpService smtpService, IUserRepository userRepository)
    {
        _emailVerificationCodeRepository = emailVerificationCodeRepository;
        _userContextService = userContextService;
        _codeHasher = codeHasher;
        _smtpService = smtpService;
        _userRepository = userRepository;
    }

    public async Task Handle(RegenerateEmailVerificationCodeRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var user = await _userRepository.GetUserById(userId)
            ?? throw new NotFoundException("User was not found.");

        await _emailVerificationCodeRepository.RemoveCodeByUserId(userId);

        var codeValue = new Random().Next(10000, 99999).ToString();

        var code = new EmailVerificationCode()
        {
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(15),
        };

        var codeHash = _codeHasher.HashPassword(code, codeValue);

        code.CodeHash = codeHash;

        await _emailVerificationCodeRepository.AddCode(code);

        await _smtpService.SendMessage(user.Email, "Hello " + user.Username, codeValue);
    }
}