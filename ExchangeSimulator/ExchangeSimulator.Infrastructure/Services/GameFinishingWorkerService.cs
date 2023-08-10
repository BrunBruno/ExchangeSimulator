using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;

namespace ExchangeSimulator.Infrastructure.Services;

public class GameFinishingWorkerService : IGameFinishingWorkerService
{
    private readonly IGameRepository _gameRepository;

    public GameFinishingWorkerService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task FinishGames()
    {
        var gamesToFinish = await _gameRepository.GetGamesToFinish();

        foreach (var game in gamesToFinish)
        {
            await FinishGame(game);
        }
    }

    private async Task FinishGame(Game game)
    {
        foreach (var player in game.Players)
        {
            CountPlayerPoints(player, game);
        }

        game.Winner = game.Players.OrderByDescending(x => x.Points).First();
        game.Status = GameStatus.Finished;
        await _gameRepository.Update(game);
    }

    private void CountPlayerPoints(Player player, Game game)
    {
        player.Points += (player.TotalBalance + player.LockedBalance) / (game.StartingBalance * game.Players.Count); //calculates percentage of the market that player has in money

        foreach (var playerCoin in player.PlayerCoins)
        {
            var gameCoin = game.StartingCoins.First(x => x.Name == playerCoin.Name);
            player.Points += (playerCoin.TotalBalance + playerCoin.LockedBalance) / (gameCoin.TotalBalance * game.Players.Count); //calculates percentage of the market that player has in coin
        }
    }
}