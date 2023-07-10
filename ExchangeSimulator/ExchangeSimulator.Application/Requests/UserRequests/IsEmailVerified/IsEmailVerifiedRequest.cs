using MediatR;

namespace ExchangeSimulator.Application.Requests.UserRequests.IsEmailVerified;

/// <summary>
/// Request for checking if email is verified.
/// </summary>
public class IsEmailVerifiedRequest : IRequest<IsEmailVerifiedDto>
{
}