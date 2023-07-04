namespace ExchangeSimulator.Shared.Exceptions;

/// <summary>
/// Exception with 400 response code.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) {}
}