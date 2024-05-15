using ExchangeSimulator.Application.Requests.TransactionRequests.GetChartData;
using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Api.Models.Transaction;

public class GetChartDataModel
{
    public string CoinName { get; set; }

    public PeriodOfTime PeriodOfTime { get; set; }
}