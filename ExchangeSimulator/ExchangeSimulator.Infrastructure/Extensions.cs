using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF;
using ExchangeSimulator.Infrastructure.EF.Options;
using ExchangeSimulator.Infrastructure.Jwt;
using ExchangeSimulator.Infrastructure.Services;
using ExchangeSimulator.Infrastructure.Workers;
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

        services.AddHttpContextAccessor();
        services.AddPostgres(configuration);
        services.AddJwt(configuration);
        services.AddCors(options => {
            options.AddPolicy("FrontEndClient", builder => {
                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin();
            });
        });
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISmtpService, SmtpService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IGameFinishingWorkerService, GameFinishingWorkerService>();
        services.AddHostedService<GameFinishingWorker>();

        return services;
    }
}