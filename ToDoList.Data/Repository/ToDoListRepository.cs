using ToDoList.Data.Enums;
using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;

namespace ToDoList.Data.Repository
{
    public class ToDoListRepository : IToDoListRepository
    {
        private readonly WebContext _webContext;

        public ToDoListRepository(WebContext webContext)
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

        public void MarkExpiredInProgressAsFailed(DateTime now, int userId)
        {
            var expired = _webContext.Tasks
                .Where(t => t.UserId == userId
                         && t.Status == Status.InProgress
                         && t.DeadlineAt != null
                         && t.DeadlineAt <= now)
                .ToList();

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
                && t.DeadlineAt > now
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
    }

}
