using Microsoft.AspNetCore.Authorization;

namespace ExchangeSimulator.Api.Authorization.IsVerified;

/// <summary>
/// Requirement that checks if user is verified.
/// </summary>
public class IsVerifiedRequirement : IAuthorizationRequirement
{
    public bool IsVerified { get; }

    public IsVerifiedRequirement(bool isVerified)
    {
        IsVerified = isVerified;
    }
}