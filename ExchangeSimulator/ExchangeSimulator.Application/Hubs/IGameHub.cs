namespace ExchangeSimulator.Application.Hubs;

public interface IGameHub
{
    Task OrdersChanged();
    Task JoinGame(string gameName);
    Task LeaveGame(string gameName);
}