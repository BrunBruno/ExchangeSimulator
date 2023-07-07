using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSimulator.Application.Requests.VerifyEmail {
    public class VerifyEmailRequestHandler : IRequestHandler<VerifyEmailRequest> {
        private readonly IEmailVerificationCodeRepository _codeRepository;
        private readonly IUserContextService _userContext;
        private readonly IUserRepository _userRepository;

        public VerifyEmailRequestHandler(IEmailVerificationCodeRepository codeRepository, IUserContextService userContext, IUserRepository userRepository) {
            _codeRepository = codeRepository;
            _userContext = userContext;
            _userRepository = userRepository;
        }
        public async Task Handle(VerifyEmailRequest request, CancellationToken cancellationToken) {
            var userId = _userContext.GetUserId()!.Value;
            var verificationCode = await _codeRepository.GetCodeByUserId(userId);

            if (verificationCode is null) {
                throw new NotFoundExeption("Code does not exist.");
            }

            if(verificationCode.Code != request.Code) {
                throw new BadRequestException("Code incorrect.");
            }
            
            var user = await _userRepository.GetUserById(userId);
            if (user is null) {
                throw new NotFoundExeption("User does not exist.");
            }

            user.IsVerified = true;
            await _userRepository.Update(user);
        }
    }
}
