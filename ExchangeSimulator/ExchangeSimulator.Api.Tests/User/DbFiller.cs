using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Infrastructure.EF.Contexts;

namespace ExchangeSimulator.Api.Tests.User;

public static partial class DbFiller
{
    public static async Task Init(this ExchangeSimulatorDbContext dbContext)
    {
        var roles = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "User"
                },
                new Role
                {
                    Id = 2,
                    Name = "Admin"
                },
            };
        await dbContext.Roles.AddRangeAsync(roles);
        await dbContext.SaveChangesAsync();
    }

    public static async Task AddUser(this ExchangeSimulatorDbContext dbContext)
    {
        var user = new Domain.Entities.User
        {
            Id = Guid.Parse(Constants.UserId),
            Email = "test@gmail.com",
            Username = "TestUserName",
            ImageUrl = "http://test.com",
            PasswordHash = "AQAAAAIAAYagAAAAEA5h41NvfPFBWbMJg+2IxbvZkzKdrJJEWujujwUWUWUcNN96a/tF/olbiRuTZuJbyA==" // "string"
        };
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public static async Task AddUserWithEmail(this ExchangeSimulatorDbContext dbContext, string email)
    {
        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = "TestUserName",
            ImageUrl = "http://test.com",
            PasswordHash = "AQAAAAIAAYagAAAAEA5h41NvfPFBWbMJg+2IxbvZkzKdrJJEWujujwUWUWUcNN96a/tF/olbiRuTZuJbyA==" // "string"
        };
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public static async Task AddCodeForUser(this ExchangeSimulatorDbContext dbContext)
    {
        var code = new Domain.Entities.EmailVerificationCode()
        {
            Id = Guid.Parse(Constants.UserId),
            CodeHash = "AQAAAAIAAYagAAAAENYYAzvI42t/TYNc5wQ58JFqttNZkv8S0/pZGxT/cic1PPSMcbiOsM8xi2w5HwmfOg==", // "57375"
            UserId = Guid.Parse(Constants.UserId),
            ExpirationDate = DateTime.Now.AddYears(10)
        };
        await dbContext.EmailVerificationCodes.AddAsync(code);
        await dbContext.SaveChangesAsync();
    }
}