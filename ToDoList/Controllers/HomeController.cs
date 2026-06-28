using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
            return View(new ToDoTaskViewModel());
        }

        [HttpPost]
        public IActionResult Index(ToDoTaskViewModel viewModel)
        {
            _toDoListService.CreateTask(viewModel);
            return RedirectToAction(nameof(ToDoList));
        }

        public IActionResult ToDoList()
        {
            var tasks = _toDoListService.GetAllTasks();
            return View(tasks);
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
