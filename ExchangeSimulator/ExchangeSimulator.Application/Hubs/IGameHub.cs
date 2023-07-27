namespace ExchangeSimulator.Application.Hubs;

public interface IGameHub
{
    Task OrdersChanged(string gameName);
    Task JoinGame(string gameName);
}