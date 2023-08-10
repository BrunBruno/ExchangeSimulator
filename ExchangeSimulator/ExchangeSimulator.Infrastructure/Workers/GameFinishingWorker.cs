using ExchangeSimulator.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExchangeSimulator.Infrastructure.Workers;

public class GameFinishingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public GameFinishingWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await FinishGames();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task FinishGames()
    {
        using var scope = _serviceProvider.CreateScope();
        var _workerService = scope.ServiceProvider.GetRequiredService<IGameFinishingWorkerService>();
        await _workerService.FinishGames();
    }
}