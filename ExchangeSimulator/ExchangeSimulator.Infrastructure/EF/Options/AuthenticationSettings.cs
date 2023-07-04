namespace ExchangeSimulator.Infrastructure.EF.Options;

/// <summary>
/// Settings for JWT authentication.
/// </summary>
public class AuthenticationSettings
{
    /// <summary>
    /// Jwt Key.
    /// </summary>
    public string JwtKey { get; set; }

    /// <summary>
    /// Days till Jwt token expires.
    /// </summary>
    public int JwtExpireDays { get; set; }

    /// <summary>
    /// Jwt issuer.
    /// </summary>
    public string JwtIssuer { get; set; }
}