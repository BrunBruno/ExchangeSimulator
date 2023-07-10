
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Application.Repositories;

public interface IGameRepository {
    Task CreateGame(Game game);
}

