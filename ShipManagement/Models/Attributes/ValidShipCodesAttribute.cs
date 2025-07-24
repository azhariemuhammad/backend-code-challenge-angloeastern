using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models.Attributes
{
    public class ValidShipCodesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable shipCodes)
            {
                return new ValidationResult("Invalid ship codes list.");
            }

            foreach (var code in shipCodes)
            {
                if (!ShipCodeValidation.IsValid(code, out var errorMessage))
                {
                    return new ValidationResult($"Invalid ship code '{code}': {errorMessage}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
