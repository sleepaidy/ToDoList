namespace ToDoList.Models.Home
{
    public class DashboardViewModel
    {
        public int ActiveCount { get; init; }
        public int DoneCount { get; init; }
        public int FailedCount { get; init; }
        public int TotalCount => ActiveCount + DoneCount + FailedCount;
        public IReadOnlyList<DashboardDeadlineViewModel> UpcomingDeadlines { get; init; } = Array.Empty<DashboardDeadlineViewModel>();
        public Dictionary<string, List<DashboardDayTaskViewModel>> TasksByDate { get; init; } = new();
    }

    public class DashboardDeadlineViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public DateTime DeadlineAt { get; init; }
        public string PriorityClass { get; init; } = string.Empty;
    }

    public class DashboardDayTaskViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string PriorityClass { get; init; } = string.Empty;
        public string StatusClass { get; init; } = string.Empty;
    }
}
