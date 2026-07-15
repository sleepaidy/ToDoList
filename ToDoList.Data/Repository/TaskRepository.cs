using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ToDoList.Data.Enums;
using ToDoList.Data.HelperModels;
using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;

namespace ToDoList.Data.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly WebContext _webContext;

        public TaskRepository(WebContext webContext)
        {
            _webContext = webContext;
        }

        public void Create(TaskData model)
        {
            _webContext.Tasks.Add(model);
            _webContext.SaveChanges();
        }

        public void Remove(TaskData model)
        {
            _webContext.Tasks.Remove(model);
            _webContext.SaveChanges();
        }

        public List<TaskData> GetAllTaskForCurrentUser(int userId)
        {
            return _webContext.Tasks
                .Where(x => x.UserId == userId)
                .ToList();
        }

        public List<TaskData> GetByStatus(Status status, int userId)
        {
            return _webContext.Tasks
                .Where(t => t.Status == status && t.UserId == userId)
                .OrderByDescending(t => t.IsImportant)
                .ThenBy(t => t.SortOrder)
                .ThenByDescending(t => t.Priority)
                .ThenBy(t => t.CreateAt)
                .ToList();
        }

        public TaskData? GetById(int id, int userId)
        {
            return _webContext.Tasks.FirstOrDefault(x => x.Id == id && x.UserId == userId);
        }

        public void UpdateStatus(TaskData task, Status status)
        {
            task.Status = status;
            _webContext.SaveChanges();
        }

        public void MarkExpiredInProgressAsFailed(DateTime now, int? userId = null)
        {
            var query = _webContext.Tasks
                    .Where(t => t.Status == Status.InProgress
                             && t.DeadlineAt != null
                             && t.DeadlineAt <= now);
            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }
            var expired = query.ToList();
            foreach (var task in expired)
            {
                task.Status = Status.Failed;
            }
            if (expired.Count > 0)
            {
                _webContext.SaveChanges();
            }
        }
        public int GetNextSortOrder(int userId, bool isImportant, Status status)
        {
            var max = _webContext.Tasks
                .Where(t => t.UserId == userId && t.Status == status && t.IsImportant == isImportant)
                .Select(t => (int?)t.SortOrder)
                .Max();
            return (max ?? -1) + 1;
        }

        public bool MoveTask(int taskId, int userId, bool moveUp)
        {
            var task = GetById(taskId, userId);
            if (task == null || task.Status != Status.InProgress)
            {
                return false;
            }
            var zone = _webContext.Tasks
                .Where(t => t.UserId == userId
                         && t.Status == Status.InProgress
                         && t.IsImportant == task.IsImportant)
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.Id)
                .ToList();
            var index = zone.FindIndex(t => t.Id == taskId);
            if (index < 0)
            {
                return false;
            }
            var swapIndex = moveUp ? index - 1 : index + 1;
            if (swapIndex < 0 || swapIndex >= zone.Count)
            {
                return false;
            }
            (zone[index], zone[swapIndex]) = (zone[swapIndex], zone[index]);
            for (var i = 0; i < zone.Count; i++)
            {
                zone[i].SortOrder = i;
            }
            _webContext.SaveChanges();
            return true;
        }

        public List<TaskData> GetTasksNeeding24HoursReminder(DateTime now)
        {
            return _webContext.Tasks
                .Where(t => t.Status == Status.InProgress
                && t.DeadlineAt != null
                && t.DeadlineAt > now.AddHours(1)
                && t.DeadlineAt <= now.AddHours(24)
                && !t.Notified24HoursBefore)
                .ToList();
        }

        public List<TaskData> GetTasksNeeding1HourReminder(DateTime now)
        {
            return _webContext.Tasks
                .Where(t => t.Status == Status.InProgress
                && t.DeadlineAt != null
                && t.DeadlineAt > now
                && t.DeadlineAt <= now.AddHours(1)
                && !t.Notified1HourBefore)
                .ToList();
        }

        public void Mark24HoursReminderSent(int taskId)
        {
            var task = _webContext.Tasks.Find(taskId);
            if(task != null)
            {
                task.Notified24HoursBefore = true;
                _webContext.SaveChanges();
            }
        }

        public void Mark1HourReminderSent(int taskId)
        {
            var task = _webContext.Tasks.Find(taskId);
            if(task != null)
            {
                task.Notified1HourBefore = true;
                _webContext.SaveChanges();  
            }
        }

        public List<string> GetDistinctCategories (Status status, int userId)
        {
            return _webContext.Tasks
                .Where(t => t.Status == status && t.UserId == userId)
                .Where(t => t.Category != null && t.Category != "")
                .Select(t => t.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public List<TaskData> GetByStatusFiltered(Status status, int userId, TaskListFilter filter)
        {
            var query = _webContext.Tasks
                .Where(t => t.Status == status && t.UserId == userId);

            if(!string.IsNullOrWhiteSpace(filter.Category))
            {
                var category = filter.Category.Trim();
                query = query.Where(t => t.Category == category);
            }

            if (filter.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.Priority.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(term));
            }

            query = ApplySorting(query, filter.SortBy, filter.SortDir);

            return query.ToList();
        }

        private IQueryable<TaskData> ApplySorting(IQueryable<TaskData> query, string? sortBy, string? sortDir)
        {
            var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
            if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
            {
                return desc
                    ? query.OrderByDescending(t => t.Name)
                    : query.OrderBy(t => t.Name);
            }
            if (string.Equals(sortBy, "Priority", StringComparison.OrdinalIgnoreCase))
            {
                return desc
                    ? query.OrderByDescending(t => t.Priority)
                    : query.OrderBy(t => t.Priority);
            }
            if (string.Equals(sortBy, "DeadlineAt", StringComparison.OrdinalIgnoreCase))
            {
                return desc
                    ? query.OrderBy(t => t.DeadlineAt == null).ThenByDescending(t => t.DeadlineAt)
                    : query.OrderBy(t => t.DeadlineAt == null).ThenBy(t => t.DeadlineAt);
            }

            return desc
                ? query.OrderByDescending(t => t.CreateAt)
                : query.OrderBy(t => t.CreateAt);
        }
    }

}
