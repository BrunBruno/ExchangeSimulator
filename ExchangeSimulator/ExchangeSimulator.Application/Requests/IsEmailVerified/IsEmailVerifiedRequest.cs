using MediatR;

namespace ExchangeSimulator.Application.Requests.IsEmailVerified;

/// <summary>
/// Request for checking if email is verified.
/// </summary>
public class IsEmailVerifiedRequest : IRequest<IsEmailVerifiedDto>
{
}