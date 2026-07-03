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
                .Where(t => t.Status == status && t.UserId == userId )
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
    }
}
