using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Enums;
using ToDoList.Models;
using ToDoList.Models.Home;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ToDoTuskViewModel viewModel)
        {
            return View();
        }

        public IActionResult ToDoList()
        {
            return View();
        }

        public IActionResult DoneList()
        {
            return View();
        }

        public IActionResult FailedList()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
