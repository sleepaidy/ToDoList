using ToDoList.Data.Enums;
using ToDoList.Data.Models;

namespace ToDoList.Data.Repository.Interfaces
{
    public interface IToDoListRepository
    {
        void Create(TaskData model);
        List<TaskData> GetAllTaskForCurrentUser(int userId);
        List<TaskData> GetByStatus(Status status, int userId);
        void Remove(TaskData model);
        TaskData? GetById(int id, int userId);
        void UpdateStatus(TaskData task, Status status);
        void MarkExpiredInProgressAsFailed(DateTime now, int userId);
    }
}