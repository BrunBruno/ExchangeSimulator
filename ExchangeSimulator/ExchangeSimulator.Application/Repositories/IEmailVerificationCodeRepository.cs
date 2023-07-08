using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

/// <summary>
/// Interface for email verification code repository.
/// </summary>
public interface IEmailVerificationCodeRepository
{
    /// <summary>
    /// Adds code.
    /// </summary>
    /// <param name="code">Email verification code.</param>
    /// <returns></returns>
    Task AddCode(EmailVerificationCode code);

    /// <summary>
    /// Gets verification code for user.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <returns></returns>
    Task<EmailVerificationCode?> GetCodeByUserId(Guid userId);

    /// <summary>
    /// Removes verification code.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RemoveCodeByUserId(Guid userId);
}