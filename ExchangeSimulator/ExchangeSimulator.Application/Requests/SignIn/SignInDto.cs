namespace ExchangeSimulator.Application.Requests.SignIn;

/// <summary>
/// DTO returned after signing in.
/// </summary>
public class SignInDto
{
    /// <summary>
    /// Jwt token used for authorization.
    /// </summary>
    public string Token { get; set; }
}