using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PartTimeJobManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEmployerRepository _employerRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            IEmployerRepository employerRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _employerRepository = employerRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            // Kiểm tra email đã tồn tại
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Lấy RoleId
            int roleId = dto.Role.ToLower() switch
            {
                "admin" => 1,
                "student" => 2,
                "employer" => 3,
                _ => throw new ArgumentException("Invalid role")
            };

            // Tạo User
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = roleId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Tạo Student hoặc Employer tùy role
            if (roleId == 2 && !string.IsNullOrEmpty(dto.FullName))
            {
                var student = new Student
                {
                    StudentId = createdUser.UserId,
                    FullName = dto.FullName
                };
                await _studentRepository.AddAsync(student);
            }
            else if (roleId == 3 && !string.IsNullOrEmpty(dto.CompanyName))
            {
                var employer = new Employer
                {
                    EmployerId = createdUser.UserId,
                    CompanyName = dto.CompanyName,
                    IsVerified = false
                };
                await _employerRepository.AddAsync(employer);
            }

            // Lấy thông tin user với role
            var userWithRole = await _userRepository.GetUserWithRoleAsync(createdUser.UserId);

            return new AuthResponseDTO
            {
                UserId = createdUser.UserId,
                Email = createdUser.Email,
                Role = userWithRole?.Role.RoleName ?? dto.Role,
                Token = GenerateJwtToken(createdUser, userWithRole?.Role.RoleName ?? dto.Role),
                ExpiresAt = DateTime.Now.AddHours(24)
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is inactive");
            }

            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.RoleName,
                Token = GenerateJwtToken(user, user.Role.RoleName),
                ExpiresAt = DateTime.Now.AddHours(24)
            };
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserWithRoleAsync(userId);

            if (user == null)
                return null;

            string? fullName = null;
            string? phone = null;

            // Lấy thông tin bổ sung dựa trên role
            if (user.Student != null)
            {
                fullName = user.Student.FullName;
                phone = user.Student.PhoneNumber;
            }
            else if (user.Employer != null)
            {
                fullName = user.Employer.ContactName ?? user.Employer.CompanyName;
                phone = user.Employer.PhoneNumber;
            }

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.RoleName,
                RoleName = user.Role.RoleName,
                FullName = fullName,
                Phone = phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }

        private string GenerateJwtToken(User user, string roleName)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456";
            var issuer = jwtSettings["Issuer"] ?? "PartTimeJobManagementAPI";
            var audience = jwtSettings["Audience"] ?? "PartTimeJobManagementClient";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
