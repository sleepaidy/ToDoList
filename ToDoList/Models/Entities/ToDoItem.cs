using ToDoList.Enums;

namespace ToDoList.Models.Entities
{
    public class ToDoItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DeadlineAt { get; set; }
        public Priority Priority { get; set; }
        public string Caregory { get; set; }
        public bool IsImportant { get; set; }
        public Status Status { get; set; }
    }
}
