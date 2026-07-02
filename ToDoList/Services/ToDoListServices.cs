using ToDoList.Data.Enums;
using ToDoList.Interfaces;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;
using System.Linq;
using System.Linq.Expressions;


namespace ToDoList.Services
{
    public class ToDoListServices : IToDoListService
    {
        private readonly List<ToDoItem> _tasks = new();
        private int _nextId = 0;
        public ToDoItem CreateTask(ToDoTaskViewModel viewModel)
        {
            ToDoItem newTask = new ToDoItem();
            newTask.Id = ++_nextId;
            newTask.Name = viewModel.Name;
            newTask.Description = viewModel.Description;
            newTask.CreateAt = DateTime.Now;
            newTask.Priority = viewModel.Priority;
            newTask.Status = viewModel.Status;
            newTask.IsImportant = viewModel.IsImportant;
            newTask.Category = viewModel.Category;
            if (viewModel.DeadlineDate == null && viewModel.DeadlineTime == null)
            {
                newTask.DeadlineAt = null;
            }
            else if (viewModel.DeadlineDate == null)
            {
                newTask.DeadlineAt = DateTime.Today + viewModel.DeadlineTime!.Value.ToTimeSpan();
            }
            else if (viewModel.DeadlineTime == null)
            {
                newTask.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                newTask.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(viewModel.DeadlineTime.Value);
            }

            _tasks.Add(newTask);
            UpdateTaskStatus();
            return newTask;
        }

        public IReadOnlyList<ToDoItem> GetAllTasks()
        {
            UpdateTaskStatus();
            return _tasks;
        }

        public IReadOnlyList<ToDoItem> GetTasksByStatus(Status status)
        {
            UpdateTaskStatus();
            return _tasks.Where(task => task.Status == status).ToList();
        }

        private void UpdateTaskStatus()
        {
            var nowTime = DateTime.Now;
            foreach (var task in _tasks)
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
                }
            }
        }

        public bool DeleteTask(int id)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);
            if (task == null)
            {
                return false;
            }
            _tasks.Remove(task);
            return true;
        }

        public bool CompleteTask(int id)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);
            if(task == null || task.Status == Status.Done)
            {
                return false;
            }
            task.Status = Status.Done;
            return true;
            
        }
    }
}
