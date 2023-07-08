using ExchangeSimulator.Application.Services;

namespace ExchangeSimulator.Api.Tests;

public class TestSmtpService : ISmtpService
{
    public async Task SendMessage(string email, string subject, string message)
    {
    }
}