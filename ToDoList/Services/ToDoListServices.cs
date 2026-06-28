using ToDoList.Interfaces;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;


namespace ToDoList.Services
{
    public class ToDoListServices : IToDoListService
    {
        private readonly List<ToDoItem> _tasks = new();
        private int _nextId = 0;
        public ToDoItem CreateTusk(ToDoTuskViewModel viewModel)
        {
            ToDoItem newTusk = new ToDoItem();
            newTusk.Id = ++_nextId;
            newTusk.Name = viewModel.Name;
            newTusk.Description = viewModel.Description;
            newTusk.CreateAt = DateTime.Now;
            newTusk.Priority = viewModel.Priority;
            newTusk.Status = viewModel.Status;
            newTusk.IsImportant = viewModel.IsImportant;
            newTusk.Category = viewModel.Category;
            if (viewModel.DeadlineDate == null && viewModel.DeadlineTime == null)
            {
                newTusk.DeadlineAt = null;
            }
            else if (viewModel.DeadlineDate == null)
            {
                newTusk.DeadlineAt = DateTime.Today + viewModel.DeadlineTime!.Value.ToTimeSpan();
            }
            else if (viewModel.DeadlineTime == null)
            {
                newTusk.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                newTusk.DeadlineAt = viewModel.DeadlineDate.Value.ToDateTime(viewModel.DeadlineTime.Value);
            }

            _tasks.Add(newTusk);
            return newTusk;
        }
    }
}
