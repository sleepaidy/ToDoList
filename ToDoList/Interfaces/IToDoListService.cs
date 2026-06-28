using ToDoList.Models.Entities;
using ToDoList.Models.Home;

namespace ToDoList.Interfaces
{
    public interface IToDoListService
    {
        ToDoItem CreateTask(ToDoTaskViewModel viewModel);
        IReadOnlyList<ToDoItem> GetAllTasks();
    }
}
