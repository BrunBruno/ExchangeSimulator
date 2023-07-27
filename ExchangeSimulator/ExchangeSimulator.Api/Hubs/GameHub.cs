using ExchangeSimulator.Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulator.Api.Hubs;

public class GameHub : Hub<IGameHub>
{
    public async Task JoinGame(string gameName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{gameName}");
    }
}