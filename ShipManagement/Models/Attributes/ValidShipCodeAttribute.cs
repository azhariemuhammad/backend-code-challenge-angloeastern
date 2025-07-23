using System.Text.RegularExpressions;

namespace ShipManagement.Models.Attributes
{
    public class ValidShipCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!ShipCodeValidation.IsValid(value, out var errorMessage))
            {
                return new ValidationResult(errorMessage);
            }
            return ValidationResult.Success;
        }
    }

    public static class ShipCodeValidation
    {
        public static readonly Regex ShipCodePattern = new Regex(@"^AE-\d{3}$", RegexOptions.Compiled);
        public const string REQUIRED_MESSAGE = "Ship code is required.";
        public const string PATTERN_MESSAGE = "Ship code must follow the pattern 'AE-XXX' where XXX is a 3-digit number (e.g., AE-001, AE-004).";

        public static bool IsValid(object? value, out string errorMessage)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                errorMessage = REQUIRED_MESSAGE;
                return false;
            }
            if (value is string shipCode && ShipCodePattern.IsMatch(shipCode))
            {
                errorMessage = string.Empty;
                return true;
            }
            errorMessage = PATTERN_MESSAGE;
            return false;
        }
    }
}
