using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models.DTOs;
using PartTimeJobManagement.Client.Services;

namespace PartTimeJobManagement.Client.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IAuthService _authService;
        private readonly IApplicationService _applicationService;
        private readonly IJobCategoryService _jobCategoryService;
        private readonly IEmployerService _employerService;
        private readonly IStudentService _studentService;

        public JobsController(
            IJobService jobService, 
            IAuthService authService, 
            IApplicationService applicationService, 
            IJobCategoryService jobCategoryService, 
            IEmployerService employerService,
            IStudentService studentService)
        {
            _jobService = jobService;
            _authService = authService;
            _applicationService = applicationService;
            _jobCategoryService = jobCategoryService;
            _employerService = employerService;
            _studentService = studentService;
        }

        // GET: Jobs
        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            IEnumerable<JobResponseDTO>? jobs;

            if (categoryId.HasValue)
            {
                jobs = await _jobService.GetJobsByCategoryAsync(categoryId.Value);
            }
            else
            {
                // Hiển thị toàn bộ jobs nếu không lọc theo category
                jobs = await _jobService.GetAllJobsAsync();
            }

            if (!string.IsNullOrEmpty(search) && jobs != null)
            {
                jobs = jobs.Where(j => 
                    j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    j.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (j.Location != null && j.Location.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Load categories for filter dropdown
            var categories = await _jobCategoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            return View(jobs ?? Enumerable.Empty<JobResponseDTO>());
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);

            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if current student has already applied
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = _authService.GetCurrentUserRole();
            
            if (role == "Student" && userId.HasValue)
            {
                var student = await _studentService.GetStudentByUserIdAsync(userId.Value);
                if (student != null)
                {
                    var applications = await _applicationService.GetApplicationsByStudentAsync(student.StudentId);
                    ViewBag.HasApplied = applications?.Any(a => a.JobId == id) ?? false;
                }
                else
                {
                    ViewBag.HasApplied = false;
                }
            }
            else
            {
                ViewBag.HasApplied = false;
            }

            return View(job);
        }

        // GET: Jobs/MyJobs (for employers)
        public async Task<IActionResult> MyJobs()
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

            // Lấy employer theo userId rồi gọi theo employer.EmployerId để khớp API
            var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);
            var jobs = employer != null
                ? await _jobService.GetJobsByEmployerAsync(employer.EmployerId)
                : Enumerable.Empty<JobResponseDTO>();

            return View(jobs ?? Enumerable.Empty<JobResponseDTO>());
        }

        // GET: Jobs/Create
        public async Task<IActionResult> Create()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", "Jobs") });
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Employer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Load categories for dropdown
            var categories = await _jobCategoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            return View();
        }

        // POST: Jobs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobDTO model)
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
                // Load categories lại cho dropdown khi có lỗi validation
                var categories = await _jobCategoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                
                TempData["ErrorMessage"] = "Please check all required fields.";
                return View(model);
            }

            // Gán EmployerId theo user hiện tại để khớp API
            var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);
            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            model.EmployerId = employer.EmployerId;
            
            // Xử lý CategoryId nếu là 0 thì set null
            if (model.CategoryId == 0)
            {
                model.CategoryId = null;
            }

            var result = await _jobService.CreateJobAsync(model);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Job posted successfully!";
                return RedirectToAction(nameof(MyJobs));
            }

            // Load categories lại cho dropdown
            var categoriesRetry = await _jobCategoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categoriesRetry;
            
            TempData["ErrorMessage"] = "Failed to create job. Please try again.";
            return View(model);
        }

        // POST: Jobs/Apply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int jobId, string? coverLetter)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Jobs", new { id = jobId }) });
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Student")
            {
                TempData["ErrorMessage"] = "Only students can apply for jobs.";
                return RedirectToAction("Details", new { id = jobId });
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy Student record để có StudentId đúng
            var student = await _studentService.GetStudentByUserIdAsync(userId.Value);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found. Please complete your profile first.";
                return RedirectToAction("StudentProfile", "Profile");
            }

            var applicationDto = new CreateApplicationDTO
            {
                StudentId = student.StudentId,
                JobId = jobId,
                CoverLetter = coverLetter
            };

            var result = await _applicationService.CreateApplicationAsync(applicationDto);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Application submitted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to submit application. You may have already applied for this job.";
            }

            return RedirectToAction("Details", new { id = jobId });
        }

        // GET: Jobs/MyApplications (for students)
        public async Task<IActionResult> MyApplications()
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

            // Lấy Student record để có StudentId đúng
            var student = await _studentService.GetStudentByUserIdAsync(userId.Value);
            if (student == null)
            {
                TempData["InfoMessage"] = "Please complete your student profile first.";
                return RedirectToAction("StudentProfile", "Profile");
            }

            var applications = await _applicationService.GetApplicationsByStudentAsync(student.StudentId);

            return View(applications ?? Enumerable.Empty<ApplicationResponseDTO>());
        }

        // POST: Jobs/WithdrawApplication/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawApplication(int id)
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

            var result = await _applicationService.DeleteApplicationAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Application withdrawn successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to withdraw application.";
            }

            return RedirectToAction(nameof(MyApplications));
        }

        // POST: Jobs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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

            return RedirectToAction(nameof(MyJobs));
        }
    }
}
