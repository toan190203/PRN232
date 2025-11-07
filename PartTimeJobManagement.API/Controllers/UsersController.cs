using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly PartTimeJobManagementContext _context;

        public UsersController(PartTimeJobManagementContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Employer)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FullName = u.Student != null ? u.Student.FullName : 
                               u.Employer != null ? u.Employer.CompanyName : 
                               u.Email,
                    Phone = u.Student != null ? u.Student.PhoneNumber : 
                            u.Employer != null ? u.Employer.PhoneNumber : 
                            null,
                    Role = u.Role.RoleName,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDTO>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Employer)
                .Where(u => u.UserId == id)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FullName = u.Student != null ? u.Student.FullName : 
                               u.Employer != null ? u.Employer.CompanyName : 
                               u.Email,
                    Phone = u.Student != null ? u.Student.PhoneNumber : 
                            u.Employer != null ? u.Employer.PhoneNumber : 
                            null,
                    Role = u.Role.RoleName,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDto)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Employer)
                .FirstOrDefaultAsync(u => u.UserId == id);
                
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.Email = updateUserDto.Email;
            user.IsActive = updateUserDto.IsActive;

            // Update Student or Employer profile
            if (user.Student != null && updateUserDto.FullName != null)
            {
                user.Student.FullName = updateUserDto.FullName;
                user.Student.PhoneNumber = updateUserDto.Phone;
            }
            else if (user.Employer != null && updateUserDto.FullName != null)
            {
                user.Employer.CompanyName = updateUserDto.FullName;
                user.Employer.PhoneNumber = updateUserDto.Phone;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow deleting admin users
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if (user.RoleId == adminRole?.RoleId)
            {
                return BadRequest("Cannot delete admin users.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }

    // DTOs
    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDTO
    {
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
