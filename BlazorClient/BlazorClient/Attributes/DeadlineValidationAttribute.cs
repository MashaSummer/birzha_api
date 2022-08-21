using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
    AttributeTargets.Field, AllowMultiple = false)]
    sealed public class DeadlineValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var deadline = (DateTime)value;
            if(deadline > DateTime.UtcNow && deadline < DateTime.UtcNow.AddDays(180))
            {
                return true;
            }
            return false;
        }
        
        public override string FormatErrorMessage(string name)
        {
            return ErrorMessageString;
        }
    }
}
