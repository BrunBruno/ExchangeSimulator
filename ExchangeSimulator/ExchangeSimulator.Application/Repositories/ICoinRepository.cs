using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

public interface ICoinRepository
{
    Task<PlayerCoin?> GetPlayerCoinById(Guid id);
    Task Update(PlayerCoin coin);
}