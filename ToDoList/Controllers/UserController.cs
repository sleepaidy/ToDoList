using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Data.Enums;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Interfaces;
using ToDoList.Models.User;
using Microsoft.AspNetCore.Authentication;
using ToDoList.Services;

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
            if(user == null)
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
            user.Language  = language;
            _userRepository.Update(user);

            user = _authService.GetUser()!;

            HttpContext.SignOutAsync(AuthService.AUTH_KEY).Wait();
            _authService.SignIn(user);

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        public IActionResult UpdateAvatar (IFormFile avatar)
        {
            if(avatar == null || avatar.Length == 0)
            {
                return RedirectToAction(nameof(Profile));
            }

            var user = _authService.GetUser()!;
            var userId = user.Id;
            var avatarsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
            Directory.CreateDirectory(avatarsDir);

            var fileName = $"avatar-{userId}.jpg";
            var path = Path.Combine(avatarsDir, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                avatar.CopyTo(fileStream); // copy to our PC
            }

            user.ProfileImage = $"/images/avatars/{fileName}";
            _userRepository.Update(user);

            return RedirectToAction(nameof(Profile));
        }


    }
}
