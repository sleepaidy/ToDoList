using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Data.Enums;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Models.User;
using Microsoft.AspNetCore.Authentication;
using ToDoList.Helpers;
using ToDoList.Services;
using ToDoList.Services.Interfaces;

namespace ToDoList.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IAuthService _authService;
        public IUserRepository _userRepository;
        public IWebHostEnvironment _webHostEnvironment;

        public UserController(IUserRepository userRepository, IAuthService authService, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var user = _authService.GetUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new ProfileViewModel
            {
                UserName = user.Name,
                AvatarUrl = string.IsNullOrEmpty(user.ProfileImage) ? null : user.ProfileImage,
                Language = user.Language
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChangeLanguage(Language language)
        {
            var user = _authService.GetUser()!;
            user.Language = language;
            _userRepository.Update(user);

            user = _authService.GetUser()!;

            HttpContext.SignOutAsync(AuthService.AUTH_KEY).Wait();
            _authService.SignIn(user);

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        public IActionResult UpdateAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
            {
                return RedirectToAction(nameof(Profile));
            }

            var extension = AvatarStorageHelper.TryGetSafeExtension(avatar);
            if (extension == null)
            {
                return RedirectToAction(nameof(Profile));
            }

            var user = _authService.GetUser()!;
            user.ProfileImage = AvatarStorageHelper.SaveAvatar(
                _webHostEnvironment, user.Id, avatar, extension);
            _userRepository.Update(user);

            return RedirectToAction(nameof(Profile));
        }
    }
}
