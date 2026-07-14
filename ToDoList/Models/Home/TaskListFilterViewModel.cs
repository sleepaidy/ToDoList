using ToDoList.Data.Enums;
using ToDoList.Models.Dtos;

namespace ToDoList.Models.Home
{
    public class TaskListFilterViewModel
    {
        public string? Category { get; set; }
        public Priority? Priority { get; set; }
        public string? Search { get; set; }
        public string SortBy { get; set; } = "CreateAt";
        public string SortDir { get; set; } = "desc";
        public Status Status { get; set; }
        public bool IsFilterActive { get; set; }

        public IReadOnlyList<ToDoTaskDto> Tasks { get; set; } = Array.Empty<ToDoTaskDto>();
        public IReadOnlyList<string> AvailableCategories { get; set; } = Array.Empty<string>();

        
    }
}
