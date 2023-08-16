using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetRealizedTransactions;
public class GetRealizedTransactionsRequest : IRequest<List<GetRealizedTransactionsDto>>
{
    public string GameName { get; set; }
    public Guid RealizationId { get; set; }
}
