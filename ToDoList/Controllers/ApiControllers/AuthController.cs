using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Data.Repository.Interfaces;

namespace ToDoList.Controllers.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public bool IsLoginFree(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return true;
            }
            return _userRepository.IsNameUniq(login.Trim());
        }

    }
}
