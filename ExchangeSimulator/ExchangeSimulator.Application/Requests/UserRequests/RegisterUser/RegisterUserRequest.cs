using MediatR;

namespace ExchangeSimulator.Application.Requests.UserRequests.RegisterUser;

/// <summary>
/// Request for user register.
/// </summary>
public class RegisterUserRequest : IRequest
{
    /// <summary>
    /// Email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Confirm password.
    /// </summary>
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// Url with user image.
    /// </summary>
    public string? ImageUrl { get; set; }
}