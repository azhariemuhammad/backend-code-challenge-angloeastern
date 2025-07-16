using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShipManagement.Models.Attributes
{
    public class RequiredValidShipIdAttribute : ValidationAttribute
    {
        private static readonly Regex ShipIdPattern = new Regex(@"^AE-\d{3}$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Ship ID is required.");
            }

            if (value is string shipId && ShipIdPattern.IsMatch(shipId))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Ship ID must follow the pattern 'AE-XXX' where XXX is a 3-digit number (e.g., AE-001, AE-004).");
        }
    }
}