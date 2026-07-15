using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ToDoList.Hubs.Interfaces;
using ToDoList.Services;

namespace ToDoList.Hubs
{
    [Authorize(AuthenticationSchemes = AuthService.AUTH_KEY)]
    public class ToDoHub : Hub<IToDoHub> 
    {
        private readonly IOnlineUserTracker _onlineUsers;

        public ToDoHub(IOnlineUserTracker onlineUsers)
        {
            _onlineUsers = onlineUsers;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUsers.UserConnected(userId);
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUsers.UserDisconnected(userId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    } 
}
