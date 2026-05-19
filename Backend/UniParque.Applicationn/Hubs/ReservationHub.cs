using Microsoft.AspNetCore.SignalR;

namespace UniParque.Application.Hubs;

public class ReservationHub : Hub
{
    public async Task JoinBranch(string branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }

    public async Task LeaveBranch(string branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }

    public async Task NotifyReservationUpdated()
    {
        await Clients.All.SendAsync("ReservationUpdated");
    }
}
