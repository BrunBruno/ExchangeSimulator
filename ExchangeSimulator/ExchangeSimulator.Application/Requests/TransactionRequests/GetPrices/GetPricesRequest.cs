
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;

public class GetPricesRequest : IRequest<List<GetPricesDto>> {
    public string GameName { get; set; }
}
