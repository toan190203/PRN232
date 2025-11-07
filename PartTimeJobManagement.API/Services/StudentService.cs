using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;

        public StudentService(IStudentRepository studentRepository, IUserRepository userRepository)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<StudentResponseDTO>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToResponseDTO);
        }

        public async Task<StudentResponseDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetStudentWithApplicationsAsync(id);
            return student != null ? MapToResponseDTO(student) : null;
        }

        public async Task<StudentResponseDTO?> GetStudentByUserIdAsync(int userId)
        {
            var student = await _studentRepository.GetStudentByUserIdAsync(userId);
            return student != null ? MapToResponseDTO(student) : null;
        }

        public async Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO dto)
        {
            // Kiểm tra email đã tồn tại
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Tạo User
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 2, // Assuming 2 is Student role ID
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Tạo Student
            var student = new Student
            {
                StudentId = createdUser.UserId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Major = dto.Major,
                YearOfStudy = dto.YearOfStudy,
                Cvfile = dto.Cvfile
            };

            var createdStudent = await _studentRepository.AddAsync(student);
            createdStudent.StudentNavigation = createdUser;

            return MapToResponseDTO(createdStudent);
        }

        public async Task<StudentResponseDTO> CreateStudentProfileAsync(CreateStudentProfileDTO dto)
        {
            // Kiểm tra User có tồn tại không
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Kiểm tra Student đã tồn tại chưa
            var existingStudent = await _studentRepository.GetByIdAsync(dto.UserId);
            if (existingStudent != null)
            {
                throw new InvalidOperationException("Student profile already exists");
            }

            // Tạo Student profile
            var student = new Student
            {
                StudentId = dto.UserId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Major = dto.Major,
                YearOfStudy = dto.YearOfStudy,
                Cvfile = dto.Cvfile
            };

            var createdStudent = await _studentRepository.AddAsync(student);
            createdStudent.StudentNavigation = user;

            return MapToResponseDTO(createdStudent);
        }

        public async Task<StudentResponseDTO> UpdateStudentAsync(int id, UpdateStudentDTO dto)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found");
            }

            student.FullName = dto.FullName;
            student.PhoneNumber = dto.PhoneNumber;
            student.Major = dto.Major;
            student.YearOfStudy = dto.YearOfStudy;
            student.Cvfile = dto.Cvfile;

            await _studentRepository.UpdateAsync(student);

            return await GetStudentByIdAsync(id) ?? throw new Exception("Failed to retrieve updated student");
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                return false;
            }

            await _studentRepository.DeleteAsync(student);
            return true;
        }

        public async Task<IEnumerable<StudentResponseDTO>> GetStudentsByMajorAsync(string major)
        {
            var students = await _studentRepository.GetStudentsByMajorAsync(major);
            return students.Select(MapToResponseDTO);
        }

        private StudentResponseDTO MapToResponseDTO(Student student)
        {
            return new StudentResponseDTO
            {
                StudentId = student.StudentId,
                Email = student.StudentNavigation?.Email ?? "",
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber,
                Major = student.Major,
                YearOfStudy = student.YearOfStudy,
                Cvfile = student.Cvfile,
                IsActive = student.StudentNavigation?.IsActive ?? false,
                CreatedAt = student.StudentNavigation?.CreatedAt ?? DateTime.MinValue,
                TotalApplications = student.Applications?.Count ?? 0
            };
        }
    }
}
