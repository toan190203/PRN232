using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> GetAll()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        /// <summary>
        /// Get student by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentResponseDTO>> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(student);
        }

        /// <summary>
        /// Get students by major
        /// </summary>
        [HttpGet("major/{major}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> GetByMajor(string major)
        {
            var students = await _studentService.GetStudentsByMajorAsync(major);
            return Ok(students);
        }

        /// <summary>
        /// Get student by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentResponseDTO>> GetByUserId(int userId)
        {
            var student = await _studentService.GetStudentByUserIdAsync(userId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(student);
        }

        /// <summary>
        /// Create a new student
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentResponseDTO>> Create([FromBody] CreateStudentDTO dto)
        {
            try
            {
                var student = await _studentService.CreateStudentAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = student.StudentId }, student);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create student profile for existing user
        /// </summary>
        [HttpPost("profile")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentResponseDTO>> CreateProfile([FromBody] CreateStudentProfileDTO dto)
        {
            try
            {
                var student = await _studentService.CreateStudentProfileAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = student.StudentId }, student);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update student information
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentResponseDTO>> Update(int id, [FromBody] UpdateStudentDTO dto)
        {
            try
            {
                var student = await _studentService.UpdateStudentAsync(id, dto);
                return Ok(student);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Upload CV file for student
        /// </summary>
        [HttpPost("{id}/upload-cv")]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UploadCV(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Only PDF, DOC, and DOCX files are allowed" });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "File size cannot exceed 5MB" });
                }

                // Create uploads directory if not exists
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"{id}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update student CV path
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                {
                    return NotFound(new { message = "Student not found" });
                }

                // Delete old CV if exists
                if (!string.IsNullOrEmpty(student.Cvfile))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", student.Cvfile.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var cvUrl = $"/uploads/cvs/{fileName}";
                var updateDto = new UpdateStudentDTO
                {
                    FullName = student.FullName,
                    PhoneNumber = student.PhoneNumber,
                    Major = student.Major,
                    YearOfStudy = student.YearOfStudy,
                    Cvfile = cvUrl
                };

                await _studentService.UpdateStudentAsync(id, updateDto);

                return Ok(new { message = "CV uploaded successfully", cvUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a student
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
                return NotFound(new { message = "Student not found" });

            return NoContent();
        }
    }
}
