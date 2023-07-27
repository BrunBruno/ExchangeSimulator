namespace ExchangeSimulator.Application.Hubs;

public interface IGameHub
{
    Task OrdersChanged();
}