using ToDoList.Enums;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;


namespace ToDoList.Interfaces
{
    public interface IToDoListService
    {
        ToDoItem CreateTask(ToDoTaskViewModel viewModel);
        IReadOnlyList<ToDoItem> GetAllTasks();
        IReadOnlyList<ToDoItem> GetTasksByStatus(Status status);
        bool DeleteTask(int id);
        bool CompleteTask(int id);
    }
}
