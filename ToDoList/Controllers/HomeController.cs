using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Models;
using ToDoList.Models.Home;
using ToDoList.Interfaces;
using ToDoList.Data.Enums;
using ToDoList.Data;

namespace ToDoList.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IToDoListService _toDoListService;
        private readonly IAuthService _authService;

        public HomeController(IToDoListService toDoListService, IAuthService authService)
        {
            _toDoListService = toDoListService;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()    
        {
            return View(new ToDoTaskViewModel());
        }

        [HttpPost]
        public IActionResult Index(ToDoTaskViewModel viewModel)
        {
            var userId = _authService.GetUserId();
            var task = _toDoListService.CreateTask(viewModel, userId);
            return task.Status switch
            {
                Status.InProgress => RedirectToAction(nameof(ToDoList)),
                Status.Done => RedirectToAction(nameof(DoneList)),
                Status.Failed => RedirectToAction(nameof(FailedList)),
                _ => RedirectToAction(nameof(ToDoList))
            };
        }

        [HttpPost]
        public IActionResult Delete(int id, string returnAction)
        {
            var userId = _authService.GetUserId();
            returnAction = returnAction switch
            {
                nameof(DoneList) => nameof(DoneList),
                nameof(FailedList) => nameof(FailedList),
                _ => nameof(ToDoList)
            };
            _toDoListService.DeleteTask(id, userId);
            return RedirectToAction(returnAction);
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var userId = _authService.GetUserId();
            _toDoListService.CompleteTask(id, userId);
            return RedirectToAction(nameof(DoneList));
        }


        public IActionResult ToDoList()
        {
            var userId = _authService.GetUserId();
            var tasks = _toDoListService.GetTasksByStatus(Status.InProgress, userId);
            return View(tasks);
        }

        public IActionResult DoneList()
        {
            var userId = _authService.GetUserId();
            var tasks = _toDoListService.GetTasksByStatus(Status.Done, userId);
            return View(tasks);
        }

        public IActionResult FailedList()
        {
            var userId = _authService.GetUserId();
            var tasks = _toDoListService.GetTasksByStatus(Status.Failed, userId);
            return View(tasks);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
