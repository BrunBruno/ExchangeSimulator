using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using ExchangeSimulator.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// Implementation for email verification code repository.
/// </summary>
public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
{
    private readonly ExchangeSimulatorDbContext _dbContext;

    public EmailVerificationCodeRepository(ExchangeSimulatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    ///<inheritdoc/>
    public async Task AddCode(EmailVerificationCode code)
    {
        await _dbContext.EmailVerificationCodes.AddAsync(code);
        await _dbContext.SaveChangesAsync();
    }

    ///<inheritdoc/>
    public async Task<EmailVerificationCode?> GetCodeByUserId(Guid userId)
        => await _dbContext.EmailVerificationCodes.FirstOrDefaultAsync(x => x.UserId == userId);

    ///<inheritdoc/>
    public async Task RemoveCodeByUserId(Guid userId)
    {
        var code = await GetCodeByUserId(userId)
            ?? throw new NotFoundException("Code was not found.");

        _dbContext.EmailVerificationCodes.Remove(code);
        await _dbContext.SaveChangesAsync();
    }
}