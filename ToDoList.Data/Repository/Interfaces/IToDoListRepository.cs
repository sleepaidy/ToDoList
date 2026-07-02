using ToDoList.Data.Enums;
using ToDoList.Data.Models;

namespace ToDoList.Data.Repository.Interfaces
{
    public interface IToDoListRepository
    {
        void Create(TaskData model);
        List<TaskData> GetAll();
        List<TaskData> GetByStatus(Status status);
        void Remove(TaskData model);
        TaskData? GetById(int id);
        void UpdateStatus(TaskData task, Status status);
        void MarkExpiredInProgressAsFailed(DateTime now);
    }
}