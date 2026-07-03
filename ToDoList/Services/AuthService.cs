using ToDoList.Data.Repository.Interfaces;
using ToDoList.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ToDoList.Data.Models;

namespace ToDoList.Services
{
    public class AuthService : IAuthService
    {
        public const string AUTH_KEY = "AuthNameToDoList";
        public const string COOKIE_ID_KEY = "Id";
        public const string COOKIE_NAME_KEY = "UserName";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public void SignIn(UserData user)
        {
            var claims = new List<Claim>
            {
                new Claim(COOKIE_ID_KEY, user.Id.ToString()),
                new Claim(COOKIE_NAME_KEY, user.Name.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.AuthenticationMethod, AUTH_KEY)
            };

            var identity = new ClaimsIdentity(claims, AUTH_KEY);
            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessor.HttpContext!
                .SignInAsync(AUTH_KEY, principal)
                .Wait();
        }

        public int GetUserId()
        {
            var userIdStr = _httpContextAccessor.HttpContext!.User?.Claims
                .FirstOrDefault(x => x.Type == COOKIE_ID_KEY)?.Value;

            return userIdStr is null ? 0 : int.Parse(userIdStr);
        }

        public string? GetUserName()
        {
            return _httpContextAccessor.HttpContext!.User?.Claims
                .FirstOrDefault(x => x.Type == COOKIE_NAME_KEY)?.Value;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public UserData? GetUser()
        {
            var userId = GetUserId();
            return userId <= 0 ? null : _userRepository.Get(userId);
        }

    }
}
