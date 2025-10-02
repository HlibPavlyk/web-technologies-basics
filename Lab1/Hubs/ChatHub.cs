using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        // Static dictionary to track users in rooms
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> RoomUsers = new();
        
        // Track connection to user mapping
        private static readonly ConcurrentDictionary<string, (string UserName, string Room)> Connections = new();
        public async Task JoinRoom(string userName, string roomNumber)
        {
            // Check if username already exists in the room
            if (RoomUsers.TryGetValue(roomNumber, out var existingUsers) && 
                existingUsers.Values.Any(u => u.Equals(userName, StringComparison.OrdinalIgnoreCase)))
            {
                await Clients.Caller.SendAsync("JoinRoomFailed", "Username already exists in this room. Please choose a different name.");
                return;
            }
            
            // Add user to the group (room)
            await Groups.AddToGroupAsync(Context.ConnectionId, roomNumber);
            
            // Track user in room
            RoomUsers.AddOrUpdate(roomNumber, 
                new ConcurrentDictionary<string, string> { [Context.ConnectionId] = userName },
                (_, users) => { users[Context.ConnectionId] = userName; return users; });
            
            // Track connection
            Connections[Context.ConnectionId] = (userName, roomNumber);
            
            // Get current users list
            var usersList = RoomUsers.TryGetValue(roomNumber, out var users) 
                ? users.Values.ToList() 
                : new List<string>();
            
            // Notify all room participants about new user joining
            await Clients.Group(roomNumber).SendAsync("UserJoined", userName, DateTime.Now.ToString("HH:mm"));
            
            // Send user list update to all room participants
            await Clients.Group(roomNumber).SendAsync("UserListUpdated", usersList);
            
            // Send confirmation to the user who joined
            await Clients.Caller.SendAsync("JoinedRoom", roomNumber, usersList);
        }

        public async Task SendMessage(string userName, string message, string roomNumber)
        {
            // Send message to all room participants
            await Clients.Group(roomNumber).SendAsync("ReceiveMessage", userName, message, DateTime.Now.ToString("HH:mm"));
        }

        public async Task LeaveRoom(string userName, string roomNumber)
        {
            // Remove user from the group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomNumber);
            
            // Remove user from tracking
            await RemoveUserFromRoom(Context.ConnectionId, roomNumber, userName);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Handle unexpected disconnection
            if (Connections.TryRemove(Context.ConnectionId, out var connectionInfo))
            {
                await RemoveUserFromRoom(Context.ConnectionId, connectionInfo.Room, connectionInfo.UserName);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        private async Task RemoveUserFromRoom(string connectionId, string roomNumber, string userName)
        {
            // Remove user from room tracking
            if (RoomUsers.TryGetValue(roomNumber, out var users))
            {
                users.TryRemove(connectionId, out _);
                
                // Get updated users list
                var usersList = users.Values.ToList();
                
                // Notify other participants about user leaving
                await Clients.Group(roomNumber).SendAsync("UserLeft", userName, DateTime.Now.ToString("HH:mm"));
                
                // Send updated user list to remaining participants
                await Clients.Group(roomNumber).SendAsync("UserListUpdated", usersList);
                
                // Clean up empty room
                if (!users.Any())
                {
                    RoomUsers.TryRemove(roomNumber, out _);
                }
            }
            
            // Remove connection tracking
            Connections.TryRemove(connectionId, out _);
        }
    }
}
