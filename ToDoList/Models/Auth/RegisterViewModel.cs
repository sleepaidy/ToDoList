using System.ComponentModel.DataAnnotations;
using ToDoList.CustomValidationAttributes;

namespace ToDoList.Models.Auth
{
    public class RegisterViewModel
    {
        [Required(
            ErrorMessageResourceName = "Login_Validation_Required",
            ErrorMessageResourceType = typeof(ToDoList.Localization.Auth))]
        public string Login { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceName = "Login_Validation_Required",
            ErrorMessageResourceType = typeof(ToDoList.Localization.Auth))]
        [PasswordCheck]
        public string Password { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceName = "Login_Validation_Required",
            ErrorMessageResourceType = typeof(ToDoList.Localization.Auth))]
        [DoubleCheck]
        public string DoubleCheckPassword { get; set; } = string.Empty;
    }
}
