using System.ComponentModel.DataAnnotations;
using ToDoList.Models.Auth;

namespace ToDoList.CustomValidationAttributes
{
    public class DoubleCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var viewModel = validationContext.ObjectInstance as LoginViewModel;
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            if(viewModel.Password == viewModel.DoubleCheckPassword)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Passwords must be the same.");

        }
    }
}
