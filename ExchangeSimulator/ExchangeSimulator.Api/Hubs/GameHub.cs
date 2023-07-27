using ExchangeSimulator.Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulator.Api.Hubs;

public class GameHub : Hub<IGameHub>
{
    public async Task OrdersChanged(Guid gameId)
    {
        await Clients.Groups($"game-{gameId}").OrdersChanged();
    }

    public async Task JoinGame(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"game-{gameId}");
    }
}