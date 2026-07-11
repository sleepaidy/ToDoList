using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ToDoList.Hubs.Interfaces;
using ToDoList.Services;

namespace ToDoList.Hubs
{
    [Authorize(AuthenticationSchemes = AuthService.AUTH_KEY)]
    public class ToDoHub : Hub<IToDoHub> { } 
}
