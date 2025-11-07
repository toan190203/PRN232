using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Auth DTOs
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Student|Employer|Admin)$", ErrorMessage = "Role must be Student, Employer, or Admin")]
        public string Role { get; set; } = null!;

        // Additional info based on role
        public string? FullName { get; set; } // For Student
        public string? CompanyName { get; set; } // For Employer
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }

    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        public string NewPassword { get; set; } = null!;
    }

    public class AuthResponseDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
