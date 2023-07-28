namespace ExchangeSimulator.Shared.Exceptions;

/// <summary>
/// Exception with 401 response code.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message){}
}