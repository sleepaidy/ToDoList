namespace ToDoList.Hubs.Interfaces
{
    public interface IToDoHub
    {
        Task DeadlineApproaching(int taskId, string taskName, string deadlineFormatted, string reminderType);
    }
}
