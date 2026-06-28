using ToDoList.Enums;

namespace ToDoList.Models.Home
{
    public class ToDoTuskViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly? DeadlineDate { get; set; }
        public TimeOnly? DeadlineTime { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public string Category { get; set; }
        public bool IsImportant { get; set; }

    }
}
