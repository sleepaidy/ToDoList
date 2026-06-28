using ToDoList.Interfaces;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;


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
            return newTask;
        }

        public IReadOnlyList<ToDoItem> GetAllTasks()
        {
            return _tasks;
        }

    }
}
