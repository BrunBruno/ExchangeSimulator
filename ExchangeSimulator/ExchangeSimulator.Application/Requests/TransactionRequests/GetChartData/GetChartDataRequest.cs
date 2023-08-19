using ExchangeSimulator.Domain.Enums;
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetChartData;

public class GetChartDataRequest : IRequest<GetChartDataDto>
{
    public string GameName { get; set; }
    public string CoinName { get; set; }
    public OrderType OrderType { get; set; }
}