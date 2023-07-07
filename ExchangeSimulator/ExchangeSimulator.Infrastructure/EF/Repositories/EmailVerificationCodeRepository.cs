using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSimulator.Infrastructure.EF.Repositories {
    public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository {
        private readonly ExchangeSimulatorDbContext _dbContext;

        public EmailVerificationCodeRepository(ExchangeSimulatorDbContext dbContext) { 
            _dbContext = dbContext;
        }

        public async Task AddCode(EmailVerificationCode code) {
            await _dbContext.EmailVerificationCodes.AddAsync(code);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<EmailVerificationCode?> GetCodeByUserId(Guid userId)
            => await _dbContext.EmailVerificationCodes.FirstOrDefaultAsync(x => x.UserId == userId);
    }
}
