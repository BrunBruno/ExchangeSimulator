﻿using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// implementation of player repository
/// </summary>
public class PlayerRepository : IPlayerRepository 
{
    private readonly ExchangeSimulatorDbContext _dbContext;

    public PlayerRepository(ExchangeSimulatorDbContext dbContext) 
    {
        _dbContext = dbContext;
    }

    ///<inheritdoc/>
    public async Task CreatePlayer(Player player) 
    {
        await _dbContext.Players.AddAsync(player);
        await _dbContext.SaveChangesAsync();
    }

    ///<inheritdoc/>
    public async Task<Player?> GetPlayerByUserIdAndGameName(string gameName, Guid userId)
        => await _dbContext.Players
            .Include(x => x.Game)
            .Include(x => x.PlayerCoins)
            .FirstOrDefaultAsync(x => x.Game.Name == gameName && x.UserId == userId);
}