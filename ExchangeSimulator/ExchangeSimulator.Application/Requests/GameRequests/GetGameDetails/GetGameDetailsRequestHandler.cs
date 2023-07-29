using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.GameRequests.GetGameDetails;
public class GetGameDetailsRequestHandler : IRequestHandler<GetGameDetailsRequest, GetGameDetailsDto> 
{
    private readonly IGameRepository _gameRepository;

    public GetGameDetailsRequestHandler(IGameRepository gameRepository) {
        _gameRepository = gameRepository;
    }
    public async Task<GetGameDetailsDto> Handle(GetGameDetailsRequest request, CancellationToken cancellationToken) {

        var game = await _gameRepository.GetGameByName(request.GameName) 
            ?? throw new NotFoundException("Game not found");

        Console.WriteLine(game);

        var gameDto = new GetGameDetailsDto
        {
            Name = game.Name,
            Description = game.Description,
            TotalBalance = game.StartingBalance,
            Duration = game.Duration,
            CreatedAt = game.CreatedAt,
            StartsAt = game.StartsAt,
            EndsAt = game.EndsAt,
            Status = game.Status,
            TotalPlayers = game.TotalPlayers,
            AvailableSpots = game.TotalPlayers - game.Players.Count,
            PlayerCount = game.Players.Count,
            Players = game.Players.Select(player => new PlayerDto
            {
                Name = player.Name
            }).ToList(),
            Coins = game.StartingCoins.Select(coin => new CoinDto
            { 
                Name = coin.Name,
                ImageUrl = coin.ImageUrl
            }).ToList(),
        };

        return gameDto;
    }
}