using ExchangeSimulator.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace ExchangeSimulator.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }
}