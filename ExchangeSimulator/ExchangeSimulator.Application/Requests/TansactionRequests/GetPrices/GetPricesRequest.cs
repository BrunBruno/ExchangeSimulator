
using MediatR;

namespace ExchangeSimulator.Application.Requests.TansactionRequests.GetPrices;

public class GetPricesRequest : IRequest<List<GetPricesDto>> {
    public string GameName { get; set; }
}
