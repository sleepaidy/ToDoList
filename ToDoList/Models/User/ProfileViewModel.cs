using ToDoList.Data.Enums;

namespace ToDoList.Models.User

{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public string? AvatarUrl { get; set; }
        public Language Language { get; set; }
    }
}
