
namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;
public class GetPricesDto {
    public decimal Price { get; set; }
    public bool? HasIncreased { get; set; }
}
