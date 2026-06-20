using ToDoList.Enums;

namespace ToDoList.Models.Home
{
    public class ToDoTuskViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly DeadlineDate { get; set; }
        public TimeOnly DeadlineTime { get; set; }
        public Priority Priority { get; set; }
        public string Category { get; set; }
        public bool isTuskImportant { get; set; }
        public bool isTuckDone { get; set; }

    }
}
