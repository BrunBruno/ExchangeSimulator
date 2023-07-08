using Microsoft.AspNetCore.Authorization;

namespace ExchangeSimulator.Api.Authorization.IsVerified;

public class IsVerifiedRequirementHandler : AuthorizationHandler<IsVerifiedRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsVerifiedRequirement requirement)
    {
        var isVerified = bool.Parse(context.User.FindFirst(c => c.Type == "IsVerified").Value);

        if (isVerified == requirement.IsVerified)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}