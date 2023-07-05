using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Infrastructure.EF;
using ExchangeSimulator.Infrastructure.EF.Options;
using ExchangeSimulator.Infrastructure.Jwt;
using ExchangeSimulator.Infrastructure.Services;
using ExchangeSimulator.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeSimulator.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<SmtpOptions>("Smtp");
        services.AddSingleton(options);

        services.AddPostgres(configuration);
        services.AddJwt(configuration);
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISmtpService, SmtpService>();
        return services;
    }
}