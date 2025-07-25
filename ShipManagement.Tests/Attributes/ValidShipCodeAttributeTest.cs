
namespace ShipManagement.Tests.Attributes
{
    public class ValidShipCodeAttributeTest
    {
        [Theory]
        [InlineData("AE-001")]
        [InlineData("AE-002")]
        public void GetValidationResult_ReturnsSuccess_WhenParameterValid(string shipCode)
        {
            var attribute = new ValidShipCodeAttribute();
            var context = new ValidationContext(new object());

            var result = attribute.GetValidationResult(shipCode, context);

            result.Should().Be(ValidationResult.Success);
        }

        [Theory]
        [InlineData("INVALID CODE")] // contains space
        [InlineData("123")] // too short
        [InlineData("!@#$%")] // special chars
        public void GetValidationResult_ReturnsError_WhenParameterInvalid(string shipCode)
        {
            var attribute = new ValidShipCodeAttribute();
            var context = new ValidationContext(new object());

            var result = attribute.GetValidationResult(shipCode, context);

            result.Should().NotBeNull();
            result.ErrorMessage.Should().NotBeNull();
        }
    }
}
