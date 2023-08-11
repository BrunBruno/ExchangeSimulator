
using ExchangeSimulator.Application.Pagination;
using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.PlayerRequests.GetAllPlayer;

public class GetAllPlayerRequestHandler : IRequestHandler<GetAllPlayerRequest, PagedResult<GetAllPlayerDto>> {
    private readonly IGameRepository _gameRepository;

    public GetAllPlayerRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<PagedResult<GetAllPlayerDto>> Handle(GetAllPlayerRequest request, CancellationToken cancellationToken) {
        var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game not found.");

        var players = game.Players;

        var playerDtos = players.Select(player => new GetAllPlayerDto() {
            Name = player.Name,
            Balance = player.TotalBalance + player.LockedBalance,
            TurnOver = player.TurnOver,
            Trades = player.Trades,
            CreatedOrders = player.CreatedOrders,
        });

        var pagedResult = new PagedResult<GetAllPlayerDto>(playerDtos.ToList(), playerDtos.Count(), request.ElementsCount, 1);

        return pagedResult;
    }
}

