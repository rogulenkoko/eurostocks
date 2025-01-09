using Microsoft.AspNetCore.SignalR;

namespace EuroStock.Domain.SignalR;

public class AppHub : Hub<IAppHubClient>
{
    private static Guid DefaultMerchant = new Guid("46570079-BEBC-4D54-BAE9-7A0EC41A9111");
    
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{DefaultMerchant}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{DefaultMerchant}");

        await base.OnDisconnectedAsync(exception);
    }
}