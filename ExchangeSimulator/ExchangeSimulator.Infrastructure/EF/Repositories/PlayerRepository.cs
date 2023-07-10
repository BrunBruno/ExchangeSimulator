using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// implementation of player repository
/// </summary>
public class PlayerRepository : IPlayerRepository {

    ///<inheritdoc/>
    public Task CreatePlayer(Player layer) {
        throw new NotImplementedException();
    }
}

