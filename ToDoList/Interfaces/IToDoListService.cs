using ToDoList.Data.Enums;
using ToDoList.Models.Entities;
using ToDoList.Models.Home;


namespace ToDoList.Interfaces
{
    public interface IToDoListService
    {
        ToDoItem CreateTask(ToDoTaskViewModel viewModel, int userId);
        IReadOnlyList<ToDoItem> GetAllTasksForCurrentUser(int userId);
        IReadOnlyList<ToDoItem> GetTasksByStatus(Status status, int userId);
        bool DeleteTask(int id, int UserId);
        bool CompleteTask(int id, int userId);
    }
}
