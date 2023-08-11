
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;

public class GetPricesRequest : IRequest<GetPricesDto?> {
    public string GameName { get; set; }
    public string CoinName { get; set; }
}
