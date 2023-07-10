using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

public interface IPlayerRepository {
    Task CreatePlayer(Player layer);
}

