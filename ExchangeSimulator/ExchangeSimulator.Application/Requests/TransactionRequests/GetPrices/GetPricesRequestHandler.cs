
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

        var transactions = game.Transactions
            .Where(t => t.CoinName == request.CoinName)
            .OrderByDescending(t => t.MadeOn)
            .Take(2);

        if (!transactions.Any()) {
            return null;
        }

        var price = new GetPricesDto();

        switch (transactions.Count())
        {
            case 1:
                price.HasIncreased = null;
                price.Price = transactions.First().Price;
                break;
            case 2:
                var priceDifference = transactions.First().Price - transactions.Last().Price;
                price.HasIncreased = priceDifference == 0 ? null : (priceDifference > 0);
                price.Price = transactions.First().Price;
                break;
        }

        return price;
    }
}
