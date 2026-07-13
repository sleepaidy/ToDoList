using ToDoList.Data.Enums;

namespace ToDoList.Data.HelperModels
{
    public class TaskListFilter
    {
        public string? Category { get; set; }
        public Priority? Priority { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
    }
}
