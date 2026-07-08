using ToDoList.Data.Enums;
using ToDoList.Models.Dtos;
using ToDoList.Models.Home;


namespace ToDoList.Interfaces
{
    public interface IToDoListService
    {
        ToDoTaskDto CreateTask(CreateToDoTaskViewModel viewModel, int userId);
        IReadOnlyList<ToDoTaskDto> GetAllTasksForCurrentUser(int userId);
        IReadOnlyList<ToDoTaskDto> GetTasksByStatus(Status status, int userId);
        bool DeleteTask(int id, int UserId);
        bool CompleteTask(int id, int userId);
        bool MoveTaskUp(int id, int userId);
        bool MoveTaskDown(int id, int userId);
    }
}
