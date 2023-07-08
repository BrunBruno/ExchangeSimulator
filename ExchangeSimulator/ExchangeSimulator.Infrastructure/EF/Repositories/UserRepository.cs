using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSimulator.Infrastructure.EF.Repositories;

/// <summary>
/// Implementation for user repository.
/// </summary>
public class UserRepository : IUserRepository {
    private readonly ExchangeSimulatorDbContext _dbContext;

    public UserRepository(ExchangeSimulatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    ///<inheritdoc/>
    public async Task<User?> GetUserByEmail(string email)
        => await _dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email);

    ///<inheritdoc/>
    public async Task AddUser(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    ///<inheritdoc/>
    public async Task<User?> GetUserById(Guid id)
        => await _dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

    ///<inheritdoc/>
    public async Task Update(User user) {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}