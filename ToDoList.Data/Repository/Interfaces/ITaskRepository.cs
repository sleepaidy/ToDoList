using ToDoList.Data.Enums;
using ToDoList.Data.HelperModels;
using ToDoList.Data.Models;

namespace ToDoList.Data.Repository.Interfaces
{
    public interface ITaskRepository
    {
        void Create(TaskData model);
        List<TaskData> GetAllTaskForCurrentUser(int userId);
        List<TaskData> GetByStatus(Status status, int userId);
        void Remove(TaskData model);
        TaskData? GetById(int id, int userId);
        void UpdateStatus(TaskData task, Status status);
        void MarkExpiredInProgressAsFailed(DateTime now, int? userId = null);
        int GetNextSortOrder(int userId, bool isImportant, Status status);
        bool MoveTask(int taskId, int userId, bool moveUp);
        List<TaskData> GetTasksNeeding24HoursReminder(DateTime now);
        List<TaskData> GetTasksNeeding1HourReminder(DateTime now);
        void Mark24HoursReminderSent(int taskId);
        void Mark1HourReminderSent(int taskId);
        List<string> GetDistinctCategories(Status status, int userId);
        List<TaskData> GetByStatusFiltered(Status status, int userId, TaskListFilter filter);

    }
}