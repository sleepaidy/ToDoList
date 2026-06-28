using ToDoList.Enums;

namespace ToDoList.Models.Entities
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DeadlineAt { get; set; } = null;
        public DateTime CreateAt { get; set; }
        public Priority Priority { get; set; }
        public string Category { get; set; }
        public bool IsImportant { get; set; }
        public Status Status { get; set; }

    }
}
