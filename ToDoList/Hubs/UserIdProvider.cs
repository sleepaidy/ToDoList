using Microsoft.AspNetCore.SignalR;
using ToDoList.Services;

namespace ToDoList.Hubs
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(AuthService.COOKIE_ID_KEY)?.Value;
        }

    }
}
