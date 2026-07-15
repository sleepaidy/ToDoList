using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using ToDoList.Data.Enums;

namespace ToDoList.Data.Models
{
    public class UserData
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string Password { get; set; }
        public string ProfileImage {  get; set; }
        public Language Language { get; set; } = Language.English;

        public virtual List<TaskData> Tasks { get; set; } = new();
    }
}
