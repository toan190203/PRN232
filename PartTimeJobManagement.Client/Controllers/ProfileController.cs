using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models.DTOs;
using PartTimeJobManagement.Client.Services;

namespace PartTimeJobManagement.Client.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IStudentService _studentService;
        private readonly IEmployerService _employerService;

        public ProfileController(IAuthService authService, IStudentService studentService, IEmployerService employerService)
        {
            _authService = authService;
            _studentService = studentService;
            _employerService = employerService;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role == "Student")
            {
                return RedirectToAction("StudentProfile");
            }
            else if (role == "Employer")
            {
                return RedirectToAction("EmployerProfile");
            }
            else if (role == "Admin")
            {
                return RedirectToAction("AdminProfile");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Profile/StudentProfile
        public async Task<IActionResult> StudentProfile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Student")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = await _studentService.GetStudentByUserIdAsync(userId.Value);

            if (student == null)
            {
                // Student profile doesn't exist - redirect to create one
                TempData["InfoMessage"] = "Please complete your student profile to continue.";
                return RedirectToAction("EditStudentProfile");
            }

            return View(student);
        }

        // GET: Profile/EditStudentProfile
        public async Task<IActionResult> EditStudentProfile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Student")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = await _studentService.GetStudentByUserIdAsync(userId.Value);

            if (student == null)
            {
                // New student profile - show empty form
                var model = new UpdateStudentDTO();
                return View(model);
            }

            var existingModel = new UpdateStudentDTO
            {
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber,
                Major = student.Major,
                YearOfStudy = student.YearOfStudy
            };

            return View(existingModel);
        }

        // POST: Profile/EditStudentProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentProfile(UpdateStudentDTO model)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var student = await _studentService.GetStudentByUserIdAsync(userId.Value);
            
            if (student == null)
            {
                // Create new student profile
                var createDto = new CreateStudentProfileDTO
                {
                    UserId = userId.Value,
                    FullName = model.FullName ?? "",
                    PhoneNumber = model.PhoneNumber,
                    Major = model.Major,
                    YearOfStudy = model.YearOfStudy,
                    Cvfile = model.Cvfile
                };

                var created = await _studentService.CreateStudentAsync(createDto);
                if (created != null)
                {
                    TempData["SuccessMessage"] = "Profile created successfully!";
                    return RedirectToAction("StudentProfile");
                }
                
                TempData["ErrorMessage"] = "Failed to create profile. Please try again.";
                return View(model);
            }

            // Update existing student profile
            var result = await _studentService.UpdateStudentAsync(student.StudentId, model);

            if (result)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("StudentProfile");
            }

            TempData["ErrorMessage"] = "Failed to update profile.";
            return View(model);
        }

        // POST: Profile/UploadCV
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCV(IFormFile cvFile)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (cvFile == null || cvFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a CV file to upload.";
                return RedirectToAction("EditStudentProfile");
            }

            var student = await _studentService.GetStudentByUserIdAsync(userId.Value);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToAction("EditStudentProfile");
            }

            var (success, message) = await _studentService.UploadCVAsync(student.StudentId, cvFile);

            if (success)
            {
                TempData["SuccessMessage"] = "CV uploaded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to upload CV: {message}";
            }

            return RedirectToAction("EditStudentProfile");
        }

        // GET: Profile/EmployerProfile
        public async Task<IActionResult> EmployerProfile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Employer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);

            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(employer);
        }

        // GET: Profile/EditEmployerProfile
        public async Task<IActionResult> EditEmployerProfile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Employer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);

            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var model = new UpdateEmployerDTO
            {
                CompanyName = employer.CompanyName,
                ContactName = employer.ContactName,
                PhoneNumber = employer.PhoneNumber,
                Address = employer.Address,
                TaxCode = employer.TaxCode
            };

            return View(model);
        }

        // POST: Profile/EditEmployerProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployerProfile(UpdateEmployerDTO model)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);
            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _employerService.UpdateEmployerAsync(employer.EmployerId, model);

            if (result)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("EmployerProfile");
            }

            TempData["ErrorMessage"] = "Failed to update profile.";
            return View(model);
        }

        // GET: Profile/AdminProfile
        public async Task<IActionResult> AdminProfile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var email = HttpContext.Session.GetString("UserEmail");

            var model = new AdminProfileViewModel
            {
                UserId = userId ?? 0,
                Email = email ?? "",
                Role = "Admin"
            };

            return View(model);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ChangePasswordDTO());
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirm password do not match");
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _authService.ChangePasswordAsync(userId.Value, model);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
                return View(model);
            }
        }
    }

    // ViewModel for Admin Profile
    public class AdminProfileViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
