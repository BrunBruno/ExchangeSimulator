using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Api.Models.Transaction;

public class GetChartDataModel
{
    public OrderType OrderType { get; set; }
    public string CoinName { get; set; }
}