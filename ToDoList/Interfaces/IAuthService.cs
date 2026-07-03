using ToDoList.Data.Models;

namespace ToDoList.Interfaces
{
    public interface IAuthService
    {
        void SignIn(UserData user);
        int GetUserId();
        string? GetUserName();
        bool IsAuthenticated();
        UserData? GetUser();
    }
}
