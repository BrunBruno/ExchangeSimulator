namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetChartData;

public class GetChartDataDto
{
    public string CoinName { get; set; }
    public string ImageUrl { get; set; }
    public PeriodOfTime PeriodOfTime { get; set; }
    public List<ChartBar> ChartBars { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Max in whole period.
    /// </summary>
    public decimal MaxValue { get; set; }

    /// <summary>
    /// Min in whole period.
    /// </summary>
    public decimal MinValue { get; set; }

    public class ChartBar
    {
        public decimal MaxValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal FirstValue { get; set; }
        public decimal LastValue { get; set; }
    }
}