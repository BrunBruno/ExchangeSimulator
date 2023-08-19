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

        var coinTransactions = game.Transactions.Where(x => x.CoinName == request.CoinName && x.OrderType == request.OrderType).OrderBy(x => x.MadeOn);

        var chartData = new GetChartDataDto
        {
            CoinName = request.CoinName,
            ImageUrl = coinImageUrl,
            OrderType = request.OrderType,
            ChartPoints = coinTransactions.Select(x => new GetChartDataDto.ChartPoint
            {
                Date = x.MadeOn,
                Value = x.Price / x.Quantity
            }).ToList()

        };

        return chartData;
    }
}