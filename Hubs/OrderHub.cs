using Microsoft.AspNetCore.SignalR;

namespace CafeManagement.Hubs
{
    public class OrderHub : Hub
    {
        // When a user (Customer, Staff, or Admin) connects
        public override async Task OnConnectedAsync()
        {
            // Optional: Log connection
            await base.OnConnectedAsync();
        }

        // Join a specific group (e.g., Staff, Admin, or specific CustomerId)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Leave a specific group
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Broadcast table status update
        public async Task UpdateTableStatus(int tableId, string status)
        {
            await Clients.All.SendAsync("ReceiveTableUpdate", new { tableId, status });
        }
    }
}

