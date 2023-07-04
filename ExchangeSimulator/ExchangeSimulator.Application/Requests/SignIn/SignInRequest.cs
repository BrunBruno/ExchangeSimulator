using MediatR;

namespace ExchangeSimulator.Application.Requests.SignIn;

/// <summary>
/// Request for signing in.
/// </summary>
public class SignInRequest : IRequest<string>
{
    /// <summary>
    /// Email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public string Password { get; set; }
}