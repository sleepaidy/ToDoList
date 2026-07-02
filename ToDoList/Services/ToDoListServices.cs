using ToDoList.Data.Enums;
using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Interfaces;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;


namespace ToDoList.Services
{
    public class ToDoListServices : IToDoListService
    {
        private readonly IToDoListRepository _repository;

        public ToDoListServices(IToDoListRepository repository)
        {
            _repository = repository;
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

            _repository.Create(entity);
            UpdateTaskStatus();

            var saved = _repository.GetById(entity.Id)
                ?? throw new InvalidOperationException($"Task with id {entity.Id} was not found after creation.");

            return MapToToDoItem(saved);
        }

        public IReadOnlyList<ToDoItem> GetAllTasks()
        {
            UpdateTaskStatus();
            return _repository.GetAll()
                .Select(MapToToDoItem)
                .ToList();
        }

        public IReadOnlyList<ToDoItem> GetTasksByStatus(Status status)
        {
            UpdateTaskStatus();
            return _repository.GetByStatus(status)
                .Select(MapToToDoItem)
                .ToList();
        }

        private void UpdateTaskStatus()
        {
            _repository.MarkExpiredInProgressAsFailed(DateTime.Now);
        }

        public bool DeleteTask(int id)
        {
            var task = _repository.GetById(id);
            if (task == null)
            {
                return false;
            }
            _repository.Remove(task);
            return true;
        }

        public bool CompleteTask(int id)
        {
            var task = _repository.GetById(id);
            if (task == null || task.Status == Status.Done)
            {
                return false;
            }
            _repository.UpdateStatus(task, Status.Done);
            return true;
        }

        private static ToDoItem MapToToDoItem(TaskData item) => new()
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
        };
    }
}
