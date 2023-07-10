using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// implementation of player coin repository
/// </summary>
public class PlayerCoinRepository : IPlayerCoinRepository {

    ///<inheritdoc/>
    public Task CraeteCoin(PlayerCoin coin) {
        throw new NotImplementedException();
    }
}

