using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetChartData;

public class GetChartDataRequestHandler : IRequestHandler<GetChartDataRequest, GetChartDataDto>
{
    private readonly IGameRepository _gameRepository;

    public GetChartDataRequestHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<GetChartDataDto> Handle(GetChartDataRequest request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game was not found.");

        var coinImageUrl = game.StartingCoins.FirstOrDefault(x => x.Name == request.CoinName)?.ImageUrl
                           ?? throw new NotFoundException("Coin was not found");

        var period = request.PeriodOfTime switch
        {
            PeriodOfTime.Minutes => TimeSpan.FromMinutes(15),
            PeriodOfTime.Hour => TimeSpan.FromHours(1),
            PeriodOfTime.Day => TimeSpan.FromDays(1),
            PeriodOfTime.Week => TimeSpan.FromDays(7),
            _ => throw new BadRequestException("Chosen period of time is not supported.")
        };

        var barCount = request.PeriodOfTime switch
        {
            PeriodOfTime.Minutes => 15,
            PeriodOfTime.Hour => 12,
            PeriodOfTime.Day => 24,
            PeriodOfTime.Week => 7,
            _ => throw new BadRequestException("Chosen period of time is not supported.")
        };

        var coinTransactions = game.Transactions.Where(x => x.CoinName == request.CoinName && x.MadeOn >= DateTime.UtcNow - period).OrderBy(x => x.MadeOn);

     

        var lastTransaction = game.Transactions.Except(coinTransactions).OrderBy(x => x.MadeOn).LastOrDefault();


        var chartData = new GetChartDataDto
        {
            CoinName = request.CoinName,
            ImageUrl = coinImageUrl,
            PeriodOfTime = request.PeriodOfTime,
            StartDate = DateTime.UtcNow - period,
            EndDate = DateTime.UtcNow,
            MaxValue = coinTransactions.Select(x => x.Price).OrderByDescending(x => x).FirstOrDefault(),
            MinValue = coinTransactions.Select(x => x.Price).OrderBy(x => x).FirstOrDefault(),
            ChartBars = new()
        };

        decimal lastValue = lastTransaction is not null ? lastTransaction.Price : 0;

        chartData.MaxValue = chartData.MaxValue == 0 ? lastValue : chartData.MaxValue;

        for (var i = 1; i <= barCount; i++)
        {
            var partOfPeriod = period / barCount;
            var startOfPartDate = chartData.StartDate + (i - 1) * partOfPeriod;
            var endOfPartDate = chartData.StartDate + i * partOfPeriod;

            var coinTransactionsInPeriod = coinTransactions.Where(x => x.MadeOn > startOfPartDate && x.MadeOn < endOfPartDate);

            var chartBar = new GetChartDataDto.ChartBar();

            chartBar.FirstValue = lastValue == 0 ? coinTransactionsInPeriod.Select(x => x.Price).FirstOrDefault() : lastValue;

            if (!coinTransactionsInPeriod.Any())
            {
                chartBar.LastValue = 0;
                chartBar.MaxValue = 0;
                chartBar.MinValue = 0;
            }
            else
            {
                chartBar.LastValue = coinTransactionsInPeriod.Select(x => x.Price).LastOrDefault();
                chartBar.MaxValue = coinTransactionsInPeriod.Select(x => x.Price).OrderByDescending(x => x).FirstOrDefault();
                chartBar.MinValue = coinTransactionsInPeriod.Select(x => x.Price).OrderBy(x => x).FirstOrDefault();
                lastValue = chartBar.LastValue;
            }

            chartBar.HasIncreased = chartBar.FirstValue < chartBar.LastValue ? true : (chartBar.FirstValue > chartBar.LastValue ? false : null);

            chartBar.HasIncreased = chartBar.LastValue == 0 ? null : chartBar.HasIncreased;

            chartData.ChartBars.Add(chartBar);
        }

        return chartData;
    }
}