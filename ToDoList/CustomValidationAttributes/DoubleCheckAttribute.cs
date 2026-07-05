using System.ComponentModel.DataAnnotations;
using ToDoList.Localization;
using ToDoList.Models.Auth;

namespace ToDoList.CustomValidationAttributes
{
    public class DoubleCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is not RegisterViewModel viewModel)
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrEmpty(viewModel.Password) || string.IsNullOrEmpty(viewModel.DoubleCheckPassword))
            {
                return ValidationResult.Success;
            }

            if (viewModel.Password == viewModel.DoubleCheckPassword)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(Auth.Validation_PasswordsMustMatch);
        }
    }
}
