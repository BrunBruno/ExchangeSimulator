using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using ExchangeSimulator.Infrastructure.EF.Options;
using ExchangeSimulator.Infrastructure.EF.Repositories;
using ExchangeSimulator.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeSimulator.Infrastructure.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {

        var options = configuration.GetOptions<PostgresOptions>("Postgres");

        services.AddDbContext<ExchangeSimulatorDbContext>(ctx
            => ctx.UseNpgsql(options.ConnectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IStartingCoinRepository, StartingCoinRepository>();
        services.AddScoped<IPlayerCoinRepository, PlayerCoinRepository>();

        return services;
    }
}