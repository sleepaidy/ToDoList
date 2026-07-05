using System.ComponentModel.DataAnnotations;
using ToDoList.Models.Auth;

namespace ToDoList.CustomValidationAttributes
{
    public class PasswordCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var viewModel = validationContext.ObjectInstance as LoginViewModel;
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            if (viewModel.Password.Length <= 4)
            {
                return new ValidationResult("Your password is too short.");
            }
            if (!char.IsUpper(viewModel.Password[0]))
            {
                return new ValidationResult("Your password must start with a capital letter.");
            }
            if(!viewModel.Password.Any(char.IsDigit))
            {
                return new ValidationResult("Your password must contain at least one digit");
            }
            return base.IsValid(value, validationContext);
        }
    }
}
