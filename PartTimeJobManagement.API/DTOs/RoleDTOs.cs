using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Role DTOs
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string RoleName { get; set; } = null!;
    }

    public class UpdateRoleDTO
    {
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;
    }

    public class RoleResponseDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public int TotalUsers { get; set; }
    }
}
