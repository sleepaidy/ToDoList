using ToDoList.Data;
using ToDoList.Data.Enums;
using ToDoList.Data.Models;
using ToDoList.Interfaces;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;


namespace ToDoList.Services
{
    public class ToDoListServices : IToDoListService
    {
        private readonly WebContext _context;

        public ToDoListServices(WebContext context)
        {
            _context = context;
        }

        public ToDoItem CreateTask(ToDoTaskViewModel viewModel)
        {
            var entity = new TaskData();
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.CreateAt = DateTime.Now;
            entity.Priority = viewModel.Priority;
            entity.Status = viewModel.Status;
            entity.IsImportant = viewModel.IsImportant;
            entity.Category = viewModel.Category;
            if (viewModel.DeadlineDate == null && viewModel.DeadlineTime == null)
            {
                entity.DeadlineAt = null;
            }
            else if (viewModel.DeadlineDate == null)
            {
                entity.DeadlineAt = DateTime.Today + viewModel.DeadlineTime!.Value.ToTimeSpan();
            }
            else if (viewModel.DeadlineTime == null)
            {
                entity.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                entity.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(viewModel.DeadlineTime.Value);
            }

            _context.Tasks.Add(entity);
            UpdateTaskStatus();
            _context.SaveChanges();
            return new ToDoItem
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreateAt = entity.CreateAt,
                DeadlineAt = entity.DeadlineAt,
                Priority = entity.Priority,
                Status = entity.Status,
                IsImportant = entity.IsImportant,
                Category = entity.Category
            };
        }

        public IReadOnlyList<ToDoItem> GetAllTasks()
        {
            UpdateTaskStatus();
            var fromDb = _context.Tasks.ToList();
            var result = new List<ToDoItem>();
            foreach (var item in fromDb)
            {
                result.Add(new ToDoItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CreateAt = item.CreateAt,
                    DeadlineAt = item.DeadlineAt,
                    Priority = item.Priority,
                    Status = item.Status,
                    IsImportant = item.IsImportant,
                    Category = item.Category
                });
            }
            return result;

        }

        public IReadOnlyList<ToDoItem> GetTasksByStatus(Status status)
        {
            UpdateTaskStatus();
            var fromDb = _context.Tasks
                .Where(t => t.Status == status)
                .ToList();
            var result = new  List<ToDoItem>();

            foreach (var item in fromDb)
            {
                result.Add(new ToDoItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    DeadlineAt = item.DeadlineAt,
                    CreateAt = item.CreateAt,
                    Priority = item.Priority,
                    Status = item.Status,
                    IsImportant = item.IsImportant,
                    Category = item.Category
                });
            }
            return result;
        }

        private void UpdateTaskStatus()
        {
            var nowTime = DateTime.Now;
            var isChanged = false;
            foreach (var task in _context.Tasks)
            {
                if (task.Status != Status.InProgress)
                {
                    continue;
                }
                if (task.DeadlineAt == null)
                {
                    continue;
                }
                if (nowTime >= task.DeadlineAt.Value)
                {
                    task.Status = Status.Failed;
                    isChanged = true;
                }
            }
            if (isChanged)
            {
                _context.SaveChanges();
            }
        }

        public bool DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(task => task.Id == id);
            if (task == null)
            {
                return false;
            }
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        public bool CompleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(task => task.Id == id);
            if(task == null || task.Status == Status.Done)
            {
                return false;
            }
            task.Status = Status.Done;
            _context.SaveChanges(); 
            return true;
            
        }
    }
}
