using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Enums;
using ToDoList.Models;
using ToDoList.Models.Home;
using ToDoList.Interfaces;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IToDoListService _toDoListService;

        public HomeController(ILogger<HomeController> logger, IToDoListService toDoListService)
        {
            _logger = logger;
            _toDoListService = toDoListService;
        }

        [HttpGet]
        public IActionResult Index()    
        {
            return View(new ToDoTuskViewModel());
        }

        [HttpPost]
        public IActionResult Index(ToDoTuskViewModel viewModel)
        {
            _toDoListService.CreateTusk(viewModel);
            return RedirectToAction(nameof(ToDoList));
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
