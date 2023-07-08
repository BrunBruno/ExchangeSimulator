namespace ExchangeSimulator.Shared.Exceptions;

/// <summary>
/// Exception with 404 response code.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}