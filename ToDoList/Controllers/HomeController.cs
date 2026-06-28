using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Models;
using ToDoList.Models.Home;
using ToDoList.Interfaces;
using ToDoList.Enums;

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
            var task = _toDoListService.CreateTask(viewModel);
            return task.Status switch
            {
                Status.InProgress => RedirectToAction(nameof(ToDoList)),
                Status.Done => RedirectToAction(nameof(DoneList)),
                Status.Failed => RedirectToAction(nameof(FailedList))
            };
        }

        [HttpPost]
        public IActionResult Delete(int id, string returnAction)
        {
            returnAction = returnAction switch
            {
                nameof(DoneList) => nameof(DoneList),
                nameof(FailedList) => nameof(FailedList),
                _ => nameof(ToDoList)
            };
            _toDoListService.DeleteTask(id);
            return RedirectToAction(returnAction);
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            _toDoListService.CompleteTask(id);
            return RedirectToAction(nameof(DoneList));
        }


        public IActionResult ToDoList()
        {
            var tasks = _toDoListService.GetTasksByStatus(Status.InProgress);
            return View(tasks);
        }

        public IActionResult DoneList()
        {
            var tasks = _toDoListService.GetTasksByStatus(Status.Done);
            return View(tasks);
        }

        public IActionResult FailedList()
        {
            var tasks = _toDoListService.GetTasksByStatus(Status.Failed);
            return View(tasks);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
