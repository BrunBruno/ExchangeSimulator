using ExchangeSimulator.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExchangeSimulator.Infrastructure.Services;

/// <summary>
/// Implementation for service used for getting information from user context.
/// </summary>
public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    ///<inheritdoc/>
    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

    ///<inheritdoc/>
    public Guid? GetUserId() =>
        User is null ? null : (Guid?)Guid.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
}
