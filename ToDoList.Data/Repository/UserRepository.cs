using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;

namespace ToDoList.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly WebContext _webContext;

        public UserRepository(WebContext webContext)
        {
            _webContext = webContext;
        }

        public UserData? GetByNameAndPassword(string userName , string password)
        {
            var hash = GetHashByPassword(password);
            return _webContext.Users
                .FirstOrDefault(x => x.Name == userName && x.Password == hash);
        }

        public bool IsNameUniq (string userName)
        {

            return !_webContext.Users.Any(x => x.Name == userName);
        }

        public void Registration(UserData user)
        {
            var hash = GetHashByPassword(user.Password);
            user.Password = hash;

            _webContext.Users.Add(user);
            _webContext.SaveChanges();
        }

        public UserData? Get(int id)
        {
            return _webContext.Users.FirstOrDefault(x => x.Id == id);
        }

        private string GetHashByPassword(string password)
        {
            password = password.Replace("a", "o");
            return password.Substring(0, password.Length - 1);
        }


    }
}
