using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ToDoList.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly WebContext _webContext;
        private readonly PasswordHasher<UserData> _passwordHasher = new();

        public UserRepository(WebContext webContext)
        {
            _webContext = webContext;
        }

        public UserData? GetByNameAndPassword(string userName , string password)
        {
            if(string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var normalized = userName.Trim();
            var user = _webContext.Users
                .FirstOrDefault(x => x.Name == normalized);
               
            if(user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Failed
                ? null
                : user;
        }

        public bool IsNameUniq(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return true;
            }

            var normalized = userName.Trim();
            return !_webContext.Users.Any(x => x.Name == normalized);
        }

        public void Registration(UserData user)
        {
            var plainPassword = user.Password;
            user.Password = _passwordHasher.HashPassword(user, plainPassword);

            _webContext.Users.Add(user);
            _webContext.SaveChanges();
        }

        public UserData? Get(int id)
        {
            return _webContext.Users.FirstOrDefault(x => x.Id == id);
        }

        

        public void Update(UserData user)
        {
            var existing = _webContext.Users.FirstOrDefault(x =>x.Id == user.Id);

            if(existing is null)
            {
                return;
            }
            existing.ProfileImage = user.ProfileImage;
            existing.Language = user.Language;

            _webContext.SaveChanges();
        }

    }
}
