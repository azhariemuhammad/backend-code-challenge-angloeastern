using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShipManagement.Models.Attributes
{
    public class RequiredValidShipCodeAttribute : ValidationAttribute
    {
        private static readonly Regex ShipCodePattern = new Regex(@"^AE-\d{3}$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Ship code is required.");
            }

            if (value is string shipId && ShipCodePattern.IsMatch(shipId))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Ship code must follow the pattern 'AE-XXX' where XXX is a 3-digit number (e.g., AE-001, AE-004).");
        }
    }
}