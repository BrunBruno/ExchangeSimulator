using ExchangeSimulator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSimulator.Application.Repositories {
    public interface IEmailVerificationCodeRepository {
        Task AddCode(EmailVerificationCode code);
        Task<EmailVerificationCode?> GetCodeByUserId(Guid userId);
    }
}
