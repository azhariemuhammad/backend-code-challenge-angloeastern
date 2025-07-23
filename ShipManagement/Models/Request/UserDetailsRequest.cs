namespace ShipManagement.Models.Request
{
    public class UserDetailsRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }
        [MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Role must be at least 3 characters long")]
        public string Role { get; set; } = "User"; // Default role
    }
}