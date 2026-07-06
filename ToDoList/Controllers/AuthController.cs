using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Data.Enums;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthController(
            IAuthService authService,
            IUserRepository userRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _authService = authService;
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Registration(RegisterViewModel viewModel, IFormFile? avatar)
        {
            if(!_userRepository.IsNameUniq(viewModel.Login) || !ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = new UserData
            {
                Name = viewModel.Login,
                Password = viewModel.Password,
                ProfileImage = "",
                Language = Language.Russian
            };

            _userRepository.Registration(user);

            if (avatar != null && avatar.Length > 0)
            {
                var avatarsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                Directory.CreateDirectory(avatarsDir);

                var fileName = $"avatar-{user.Id}.jpg";
                var path = Path.Combine(avatarsDir, fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    avatar.CopyTo(fileStream);
                }

                user.ProfileImage = $"/images/avatars/{fileName}";
                _userRepository.Update(user);
            }

            var savedUser = _userRepository.Get(user.Id);
            if (savedUser is not null)
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
