using System.ComponentModel.DataAnnotations;
using ToDoList.Localization;

namespace ToDoList.CustomValidationAttributes
{
    public class PasswordCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string password || string.IsNullOrEmpty(password))
            {
                return ValidationResult.Success;
            }

            if (password.Length <= 4)
            {
                return new ValidationResult(Auth.Validation_PasswordTooShort);
            }

            if (!char.IsUpper(password[0]))
            {
                return new ValidationResult(Auth.Validation_PasswordMustStartUpper);
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult(Auth.Validation_PasswordMustContainDigit);
            }

            return ValidationResult.Success;
        }
    }
}
