using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Infrastructure.EF.Contexts;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeSimulator.Api.Tests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private FakeUserFilter _userFilter;

    public TestWebApplicationFactory()
    {
        _userFilter = new FakeUserFilter(true);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptions = services
                .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ExchangeSimulatorDbContext>));

            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            services.AddMvc(option => option.Filters.Add(_userFilter));

            var smtpService = services.First(s => s.ServiceType == typeof(ISmtpService));
            services.Remove(smtpService);
            services.AddScoped<ISmtpService, TestSmtpService>();

            services.Remove(dbContextOptions);
            services.AddDbContext<ExchangeSimulatorDbContext>(options => options.UseInMemoryDatabase("ExchangeSimulatorDb"));
        });
    }

    public void ChangeIsVerifiedClaim(IWebHostBuilder builder, bool isVerified)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMvc(option =>
            {
                option.Filters.Remove(_userFilter);
                option.Filters.Add(new FakeUserFilter(isVerified));
            });
        });
    }
}