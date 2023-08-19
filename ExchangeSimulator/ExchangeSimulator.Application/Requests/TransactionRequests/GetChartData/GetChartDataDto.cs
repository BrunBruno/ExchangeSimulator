using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetChartData;

public class GetChartDataDto
{
    public string CoinName { get; set; }
    public string ImageUrl { get; set; }
    public OrderType OrderType { get; set; }
    public List<ChartPoint> ChartPoints { get; set; }

    public class ChartPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}