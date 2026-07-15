namespace ToDoList.Hubs.Interfaces
{
    public interface IOnlineUserTracker
    {
        void UserConnected(string userId);
        void UserDisconnected(string userId);
        bool IsOnline(string userId);
    }
}
