using ToDoList.Data.Models;

namespace ToDoList.Data.Repository.Interfaces
{
    public interface IUserRepository
    {
        UserData? GetByNameAndPassword(string name, string password);
        bool IsNameUniq (string name);
        void Registration(UserData user);
        UserData? Get(int id);
        void Update(UserData user);
    }
}
