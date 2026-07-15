using ToDoList.Data.Enums;
using ToDoList.Data.Models;

namespace ToDoList.Services.Interfaces
{
    public interface IAuthService
    {
        void SignIn(UserData user);
        int GetUserId();
        string? GetUserName();
        bool IsAuthenticated();
        UserData? GetUser();
        Language GetLanguage(); 
    }
}
