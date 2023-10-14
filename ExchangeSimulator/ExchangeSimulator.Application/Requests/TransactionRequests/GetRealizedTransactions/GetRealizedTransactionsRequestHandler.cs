using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetRealizedTransactions;
public class GetRealizedTransactionsRequestHandler : IRequestHandler<GetRealizedTransactionsRequest, List<GetRealizedTransactionsDto>>
{
    private readonly IGameRepository _gameRepository;

    public GetRealizedTransactionsRequestHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    public async Task<List<GetRealizedTransactionsDto>> Handle(GetRealizedTransactionsRequest request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetGameByName(request.GameName)
            ?? throw new NotFoundException("Game not found.");

        var transactions = game.Transactions.Where(t => t.RealizationId == request.RealizationId);

        var transactionDtos = transactions.Select(t => new GetRealizedTransactionsDto()
        {
            MoneyAmount = t.Price * t.Quantity,
            CoinAmount = t.Quantity,
        });

        return transactionDtos.ToList();
    }
}
