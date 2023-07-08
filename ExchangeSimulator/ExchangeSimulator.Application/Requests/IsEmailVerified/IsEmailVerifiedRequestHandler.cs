using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.IsEmailVerified;

/// <summary>
/// Handler for checking if email is verified.
/// </summary>
public class IsEmailVerifiedRequestHandler : IRequestHandler<IsEmailVerifiedRequest, IsEmailVerifiedDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;

    public IsEmailVerifiedRequestHandler(IUserContextService userContextService, IUserRepository userRepository)
    {
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task<IsEmailVerifiedDto> Handle(IsEmailVerifiedRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var user = await _userRepository.GetUserById(userId)
            ?? throw new NotFoundException("User was not found.");

        return new IsEmailVerifiedDto
        {
            IsEmailVerified = user.IsVerified
        };
    }
}