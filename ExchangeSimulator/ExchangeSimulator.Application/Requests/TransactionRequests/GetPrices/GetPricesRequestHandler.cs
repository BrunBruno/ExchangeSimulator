
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;

public class GetPricesRequestHandler : IRequestHandler<GetPricesRequest, List<GetPricesDto>> {
    private readonly IGameRepository _gameRepository;

    public GetPricesRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<List<GetPricesDto>> Handle(GetPricesRequest request, CancellationToken cancellationToken) {
       var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game not found.");

        var transactions = game.Transactions
            .GroupBy(transaction => transaction.CoinName)
            .Select(group => group.OrderByDescending(transaction => transaction.MadeOn).First())
            .ToList();

        var prices = transactions.Select(transaction => new GetPricesDto(){
            Price = transaction.Price,
        }).ToList();

        return prices;
    }
}
