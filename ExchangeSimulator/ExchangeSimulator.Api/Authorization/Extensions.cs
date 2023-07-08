using ExchangeSimulator.Api.Authorization.IsVerified;
using Microsoft.AspNetCore.Authorization;

namespace ExchangeSimulator.Api.Authorization;

public static class Extensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsVerified", builder => builder.AddRequirements(new IsVerifiedRequirement(true)));
            options.AddPolicy("IsNotVerified", builder => builder.AddRequirements(new IsVerifiedRequirement(false)));
        });

        services.AddScoped<IAuthorizationHandler, IsVerifiedRequirementHandler>();
        return services;
    }
}