using System.ComponentModel.DataAnnotations;
using ToDoList.CustomValidationAttributes;

namespace ToDoList.Models.Auth
{
    public class LoginViewModel
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        [PasswordCheck]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string DoubleCheckPassword { get; set; } = string.Empty;
    }
}
