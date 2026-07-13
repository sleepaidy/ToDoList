using ToDoList.Data.Enums;
using ToDoList.Data.HelperModels;
using ToDoList.Data.Models;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Interfaces;
using ToDoList.Models.Dtos;
using ToDoList.Models.Home;


namespace ToDoList.Services
{
    public class ToDoListServices : IToDoListService
    {
        private readonly ITaskRepository _repository;

        public ToDoListServices(ITaskRepository repository)
        {
            _repository = repository;
        }

        public ToDoTaskDto CreateTask(CreateToDoTaskViewModel viewModel, int userId)
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
                entity.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(new TimeOnly(23, 59));
            }
            else
            {
                entity.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(viewModel.DeadlineTime.Value);
            }

            entity.UserId = userId;
            entity.SortOrder = _repository.GetNextSortOrder(userId, entity.IsImportant, entity.Status);
            _repository.Create(entity);
            UpdateTaskStatus(userId);

            var saved = _repository.GetById(entity.Id, userId)
                ?? throw new InvalidOperationException($"Task with id {entity.Id} was not found after creation.");

            return MapToDto(saved);
        }

        public IReadOnlyList<ToDoTaskDto> GetAllTasksForCurrentUser(int userId)
        {
            UpdateTaskStatus(userId);
            return _repository.GetAllTaskForCurrentUser(userId)
                .Select(MapToDto)
                .ToList();
        }

        public IReadOnlyList<ToDoTaskDto> GetTasksByStatus(Status status, int userId)
        {
            UpdateTaskStatus(userId);
            return _repository.GetByStatus(status, userId)
                .Select(MapToDto)
                .ToList();
        }

        private void UpdateTaskStatus(int userId)
        {
            _repository.MarkExpiredInProgressAsFailed(DateTime.Now, userId);
        }

        public bool DeleteTask(int id, int userId)
        {
            var task = _repository.GetById(id, userId);
            if (task == null)
            {
                return false;
            }
            _repository.Remove(task);
            return true;
        }

        public bool CompleteTask(int id, int userId)
        {
            var task = _repository.GetById(id, userId);
            if (task == null || task.Status != Status.InProgress)
            {
                return false;
            }
            _repository.UpdateStatus(task, Status.Done);
            return true;
        }

        private static ToDoTaskDto MapToDto(TaskData item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            CreateAt = item.CreateAt,
            DeadlineAt = item.DeadlineAt,
            Priority = item.Priority,
            Status = item.Status,
            IsImportant = item.IsImportant,
            Category = item.Category,
            SortOrder = item.SortOrder
        };

        public bool MoveTaskUp(int id, int userId)
        {
            return _repository.MoveTask(id, userId, true);
        }
        public bool MoveTaskDown(int id, int userId)
        {
            return _repository.MoveTask(id, userId, false);
        }

        public TaskListFilterViewModel GetFilteredTasksByStatus(Status status, int userId, TaskListFilter filter)
        {
            UpdateTaskStatus(userId);

            var repositoryFilter = new TaskListFilter
            {
                Category = filter.Category,
                Priority = filter.Priority,
                Search = filter.Search,
                SortBy = filter.SortBy,
                SortDir = filter.SortDir,
            };

            var categories = _repository.GetDistinctCategories(status, userId);

            var tasks = _repository
                .GetByStatusFiltered(status, userId, repositoryFilter)
                .Select(MapToDto)
                .ToList();

            return new TaskListFilterViewModel
            {
                Category = filter.Category,
                Priority = filter.Priority,
                Search = filter.Search,
                SortBy = string.IsNullOrWhiteSpace(filter.SortBy) ? "CreateAt" : filter.SortBy,
                SortDir = string.Equals(filter.SortDir, "asc", StringComparison.OrdinalIgnoreCase)
                    ? "asc"
                    : "desc",
                Status = status,
                IsFilterActive =
                    !string.IsNullOrWhiteSpace(filter.Category)
                    || filter.Priority.HasValue
                    || !string.IsNullOrWhiteSpace(filter.Search),
                Tasks = tasks,
                AvailableCategories = categories,
            };
        }
    }
}
