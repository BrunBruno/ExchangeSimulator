using System.Security.Claims;

namespace ExchangeSimulator.Application.Services;

/// <summary>
/// Service used for getting informations from user context.
/// </summary>
public interface IUserContextService
{
    /// <summary>
    /// Gets user id from user context.
    /// </summary>
    /// <returns>User id.</returns>
    Guid? GetUserId();

    /// <summary>
    /// Gets user context.
    /// </summary>
    ClaimsPrincipal User { get; }
}