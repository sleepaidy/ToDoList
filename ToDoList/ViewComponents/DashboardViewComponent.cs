using Microsoft.AspNetCore.Mvc;
using ToDoList.Data.Enums;
using ToDoList.Interfaces;
using ToDoList.Models.Home;

namespace ToDoList.ViewComponents
{
    public class DashboardViewComponent : ViewComponent
    {
        private readonly IToDoListService _toDoListService;

        public DashboardViewComponent(IToDoListService toDoListService)
        {
            _toDoListService = toDoListService;
        }

        public IViewComponentResult Invoke()
        {
            var allTasks = _toDoListService.GetAllTasks();
            var now = DateTime.Now;

            var tasksByDate = allTasks
                .Where(task => task.DeadlineAt.HasValue)
                .GroupBy(task => task.DeadlineAt!.Value.Date)
                .ToDictionary(
                    group => group.Key.ToString("yyyy-MM-dd"),
                    group => group
                        .OrderBy(task => task.DeadlineAt)
                        .Select(task => new DashboardDayTaskViewModel
                        {
                            Id = task.Id,
                            Name = task.Name,
                            PriorityClass = GetPriorityClass(task.Priority),
                            StatusClass = GetStatusClass(task.Status)
                        })
                        .ToList());

            var upcoming = allTasks
                .Where(task => task.Status == Status.InProgress && task.DeadlineAt.HasValue && task.DeadlineAt >= now)
                .OrderBy(task => task.DeadlineAt)
                .Take(6)
                .Select(task => new DashboardDeadlineViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    DeadlineAt = task.DeadlineAt!.Value,
                    PriorityClass = GetPriorityClass(task.Priority)
                })
                .ToList();

            var model = new DashboardViewModel
            {
                ActiveCount = allTasks.Count(task => task.Status == Status.InProgress),
                DoneCount = allTasks.Count(task => task.Status == Status.Done),
                FailedCount = allTasks.Count(task => task.Status == Status.Failed),
                UpcomingDeadlines = upcoming,
                TasksByDate = tasksByDate
            };

            return View(model);
        }

        private static string GetPriorityClass(Priority priority) => priority switch
        {
            Priority.High => "high",
            Priority.Medium => "medium",
            _ => "low"
        };

        private static string GetStatusClass(Status status) => status switch
        {
            Status.Done => "done",
            Status.Failed => "failed",
            _ => "active"
        };
    }
}
