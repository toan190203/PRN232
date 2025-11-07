using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Services;
using PartTimeJobManagement.Client.Models.DTOs;

namespace PartTimeJobManagement.Client.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IJobService _jobService;
        private readonly IStudentService _studentService;
        private readonly IEmployerService _employerService;
        private readonly IApplicationService _applicationService;
        private readonly IContractService _contractService;
        private readonly IPaymentService _paymentService;

        public AdminController(
            IAuthService authService,
            IUserService userService,
            IJobService jobService,
            IStudentService studentService,
            IEmployerService employerService,
            IApplicationService applicationService,
            IContractService contractService,
            IPaymentService paymentService)
        {
            _authService = authService;
            _userService = userService;
            _jobService = jobService;
            _studentService = studentService;
            _employerService = employerService;
            _applicationService = applicationService;
            _contractService = contractService;
            _paymentService = paymentService;
        }

        private IActionResult? CheckAdminAccess()
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

            return null;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var users = await _userService.GetAllUsersAsync();
            var jobs = await _jobService.GetAllJobsAsync();
            var students = await _studentService.GetAllStudentsAsync();
            var employers = await _employerService.GetAllEmployersAsync();
            var applications = await _applicationService.GetAllApplicationsAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = users?.Count() ?? 0,
                TotalStudents = students?.Count() ?? 0,
                TotalEmployers = employers?.Count() ?? 0,
                TotalJobs = jobs?.Count() ?? 0,
                ActiveJobs = jobs?.Count(j => j.Status == "Open") ?? 0,
                TotalApplications = applications?.Count() ?? 0,
                PendingApplications = applications?.Count(a => a.Status == "Pending") ?? 0,
                RecentUsers = users?.OrderByDescending(u => u.CreatedAt).Take(5).ToList() ?? new List<UserResponseDTO>(),
                RecentJobs = jobs?.OrderByDescending(j => j.PostedDate).Take(5).ToList() ?? new List<JobResponseDTO>()
            };

            return View(model);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users(string search = "", string role = "")
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(search))
            {
                users = users?.Where(u => 
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (u.FullName != null && u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                users = users?.Where(u => u.RoleName == role);
            }

            ViewBag.Search = search;
            ViewBag.Role = role;

            var userList = users?.ToList() ?? new List<UserResponseDTO>();

            return View(userList);
        }

        // GET: Admin/Jobs
        public async Task<IActionResult> Jobs(string search = "", string status = "")
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var jobs = await _jobService.GetAllJobsAsync();

            if (!string.IsNullOrEmpty(search))
            {
                jobs = jobs?.Where(j => 
                    j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    j.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                jobs = jobs?.Where(j => j.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(jobs?.ToList() ?? new List<JobResponseDTO>());
        }

        // GET: Admin/Applications
        public async Task<IActionResult> Applications(string status = "")
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var applications = await _applicationService.GetAllApplicationsAsync();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                applications = applications?.Where(a => a.Status == status);
            }

            ViewBag.Status = status;

            return View(applications?.ToList() ?? new List<ApplicationResponseDTO>());
        }

        // GET: Admin/Employers
        public async Task<IActionResult> Employers(string search = "", string verified = "")
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var employers = await _employerService.GetAllEmployersAsync();
            IEnumerable<EmployerResponseDTO>? filteredEmployers = employers;

            if (!string.IsNullOrEmpty(search))
            {
                filteredEmployers = filteredEmployers?.Where(e => 
                    (e.CompanyName != null && e.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (e.ContactName != null && e.ContactName.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(verified))
            {
                if (verified == "Verified")
                {
                    filteredEmployers = filteredEmployers?.Where(e => e.IsVerified);
                }
                else if (verified == "Pending")
                {
                    filteredEmployers = filteredEmployers?.Where(e => !e.IsVerified);
                }
            }

            ViewBag.Search = search;
            ViewBag.Verified = verified;

            return View(filteredEmployers?.ToList() ?? new List<EmployerResponseDTO>());
        }

        // POST: Admin/VerifyEmployer/{id}
        [HttpPost]
        public async Task<IActionResult> VerifyEmployer(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var employer = await _employerService.GetEmployerByIdAsync(id);
            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer not found.";
                return RedirectToAction("Employers");
            }

            var updateDto = new UpdateEmployerDTO
            {
                CompanyName = employer.CompanyName,
                ContactName = employer.ContactName,
                PhoneNumber = employer.PhoneNumber,
                Address = employer.Address,
                TaxCode = employer.TaxCode,
                IsVerified = true
            };

            var result = await _employerService.UpdateEmployerAsync(id, updateDto);

            if (result)
            {
                TempData["SuccessMessage"] = $"Employer '{employer.CompanyName}' verified successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to verify employer.";
            }

            return RedirectToAction("Employers");
        }

        // POST: Admin/UnverifyEmployer/{id}
        [HttpPost]
        public async Task<IActionResult> UnverifyEmployer(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var employer = await _employerService.GetEmployerByIdAsync(id);
            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer not found.";
                return RedirectToAction("Employers");
            }

            var updateDto = new UpdateEmployerDTO
            {
                CompanyName = employer.CompanyName,
                ContactName = employer.ContactName,
                PhoneNumber = employer.PhoneNumber,
                Address = employer.Address,
                TaxCode = employer.TaxCode,
                IsVerified = false
            };

            var result = await _employerService.UpdateEmployerAsync(id, updateDto);

            if (result)
            {
                TempData["SuccessMessage"] = $"Employer '{employer.CompanyName}' unverified.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unverify employer.";
            }

            return RedirectToAction("Employers");
        }

        // POST: Admin/ToggleUserStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Users");
            }

            // Toggle IsActive status
            var updateDto = new UpdateUserDTO
            {
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                IsActive = !user.IsActive
            };

            var result = await _userService.UpdateUserAsync(id, updateDto);

            if (result)
            {
                TempData["SuccessMessage"] = $"User {(updateDto.IsActive ? "activated" : "deactivated")} successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user status.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/DeleteUser/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var result = await _userService.DeleteUserAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/DeleteJob/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            try
            {
                var result = await _jobService.DeleteJobAsync(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Job deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete job.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Jobs");
        }
    }

    // ViewModel for Dashboard
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalEmployers { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public List<UserResponseDTO> RecentUsers { get; set; } = new();
        public List<JobResponseDTO> RecentJobs { get; set; } = new();
    }
}
