using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Services;

/// <summary>
/// Interface for jwt service.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Creates jwt token for user.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns>Token.</returns>
    string GetJwtToken(User user);
}