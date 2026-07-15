using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Enums;
using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Helpers;
using ToDoList.Localization;
using ToDoList.Models.Auth;
using ToDoList.Services;
using ToDoList.Services.Interfaces;

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

            var login = viewModel.Login?.Trim() ?? "";
            var user = _userRepository.GetByNameAndPassword(login, viewModel.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, Auth.Validation_InvalidCredentials);
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
            viewModel.Login = viewModel.Login?.Trim() ?? "";

            if (string.IsNullOrEmpty(viewModel.Login))
            {
                ModelState.Remove(nameof(RegisterViewModel.Login));
                ModelState.AddModelError(
                    nameof(RegisterViewModel.Login),
                    Auth.Login_Validation_Required);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (!_userRepository.IsNameUniq(viewModel.Login))
            {
                ModelState.AddModelError(
                    nameof(RegisterViewModel.Login),
                    Auth.Validation_LoginTaken);

                return View(viewModel);
            }

            var user = new UserData
            {
                Name = viewModel.Login,
                Password = viewModel.Password,
                ProfileImage = "",
                Language = ResolveLanguageFromCurrentCulture()
            };

            try
            {
                _userRepository.Registration(user);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    nameof(RegisterViewModel.Login),
                    Auth.Validation_LoginTaken);
                return View(viewModel);
            }

            if (avatar != null && avatar.Length > 0)
            {
                var extension = AvatarStorageHelper.TryGetSafeExtension(avatar);
                if (extension != null)
                {
                    user.ProfileImage = AvatarStorageHelper.SaveAvatar(
                        _webHostEnvironment, user.Id, avatar, extension);
                    _userRepository.Update(user);
                }
            }

            var savedUser = _userRepository.Get(user.Id);
            if (savedUser is not null)
            {
                _authService.SignIn(savedUser);
            }

            return RedirectToAction("ToDoList", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(AuthService.AUTH_KEY)
                .Wait();
            return RedirectToAction(nameof(Login));
        }

        private static Language ResolveLanguageFromCurrentCulture()
        {
            var twoLetter = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return string.Equals(twoLetter, "en", StringComparison.OrdinalIgnoreCase)
                ? Language.English
                : Language.Russian;
        }
    }
}
