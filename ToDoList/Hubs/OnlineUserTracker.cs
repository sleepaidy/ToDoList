using System.Collections.Concurrent;
using ToDoList.Hubs.Interfaces;

namespace ToDoList.Hubs
{
    public class OnlineUserTracker : IOnlineUserTracker
    {
        private readonly ConcurrentDictionary<string, int> _connections = new();

        public void UserConnected(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return;
            }
            _connections.AddOrUpdate(userId, 1, (_, count) => count + 1);
        }

        public void UserDisconnected(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }
            _connections.AddOrUpdate(userId, 0, (_, count) => Math.Max(0, count - 1));

            if(_connections.TryGetValue(userId, out var left) && left == 0)
            {
                _connections.TryRemove(userId, out _);
            }
        }

        public bool IsOnline(string userId)
        {
            return !string.IsNullOrEmpty(userId)
                && _connections.TryGetValue(userId, out var count)
                && count > 0;
        }

    }
}
