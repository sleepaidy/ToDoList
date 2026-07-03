using ToDoList.Data.Enums;

namespace ToDoList.Data.Models
{
    public class TaskData
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
        public int UserId { get; set; }

        public virtual UserData User { get; set; }
    }
}
