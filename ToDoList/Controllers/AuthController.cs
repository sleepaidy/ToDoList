using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Interfaces;
using ToDoList.Models.Auth;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = _userRepository.GetByNameAndPassword(viewModel.Login, viewModel.Password);

            if (user == null)
            {
                return View(viewModel);
            }

            _authService.SignIn(user);
            return RedirectToAction("ToDoList", "Home");
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(RegisterViewModel viewModel)
        {
            if(!_userRepository.IsNameUniq(viewModel.Login) || !ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = new UserData
            {
                Name = viewModel.Login,
                Password = viewModel.Password,
                ProfileImage = ""
            };

            _userRepository.Registration(user);

            var savedUser = _userRepository.GetByNameAndPassword(viewModel.Login, viewModel.Password);
            if(savedUser is not null)
            {
                _authService.SignIn(savedUser);
            }
            
            return RedirectToAction("ToDoList", "Home");
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(AuthService.AUTH_KEY)
                .Wait();
            return RedirectToAction(nameof(Login));
        }

    }
}
