
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.TransactionRequests.GetPrices;

public class GetPricesRequestHandler : IRequestHandler<GetPricesRequest, GetPricesDto?> {
    private readonly IGameRepository _gameRepository;

    public GetPricesRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<GetPricesDto?> Handle(GetPricesRequest request, CancellationToken cancellationToken) {
       var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game not found.");

        var transaction = game.Transactions
            .Where(t => t.CoinName == request.CoinName)
            .OrderByDescending(t => t.MadeOn)
            .FirstOrDefault();

        if (transaction is null) {
            return null;
        }

        var price = new GetPricesDto() { 
            Price = transaction.Price,
        };

        return price;
    }
}
