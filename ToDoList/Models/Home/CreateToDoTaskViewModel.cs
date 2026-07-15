using System.ComponentModel.DataAnnotations;
using ToDoList.Data.Enums;

namespace ToDoList.Models.Home
{
    public class CreateToDoTaskViewModel : IValidatableObject
    {
        [Required(
            ErrorMessageResourceName = "Index_Validation_Name_Required",
            ErrorMessageResourceType = typeof(ToDoList.Localization.Home))]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly? DeadlineDate { get; set; }
        public TimeOnly? DeadlineTime { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public string Category { get; set; }
        public bool IsImportant { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DeadlineTime.HasValue && !DeadlineDate.HasValue)
            {
                yield return new ValidationResult(
                    Localization.Home.Index_Validation_DeadlineTimeRequiresDate,
                    new[] { nameof(DeadlineTime) });
            }
        }
    }
}
