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

        public List<TaskData> GetAll()
        {
            return _webContext.Tasks.ToList();
        }

        public List<TaskData> GetByStatus(Status status)
        {
            return _webContext.Tasks
                .Where(t => t.Status == status)
                .ToList();
        }

        public TaskData? GetById(int id)
        {
            return _webContext.Tasks.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateStatus(TaskData task, Status status)
        {
            task.Status = status;
            _webContext.SaveChanges();
        }

        public void MarkExpiredInProgressAsFailed(DateTime now)
        {
            var expired = _webContext.Tasks
                .Where(t => t.Status == Status.InProgress
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
