using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeSimulator.Api.Tests;

public static class Extensions
{
    public static ExchangeSimulatorDbContext GetDbContextForAsserts(this TestWebApplicationFactory<Program> factory)
    {
        var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
        var scope = scopeFactory.CreateScope();
        var assertDbContext = scope.ServiceProvider.GetService<ExchangeSimulatorDbContext>();
        return assertDbContext;
    }
}