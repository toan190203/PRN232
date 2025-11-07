using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models.DTOs;
using PartTimeJobManagement.Client.Services;

namespace PartTimeJobManagement.Client.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IApplicationService _applicationService;
        private readonly IJobService _jobService;
        private readonly IEmployerService _employerService;

        public ApplicationsController(IAuthService authService, IApplicationService applicationService, IJobService jobService, IEmployerService employerService)
        {
            _authService = authService;
            _applicationService = applicationService;
            _jobService = jobService;
            _employerService = employerService;
        }

        // GET: Applications (for employers to see all applications for their jobs)
        public async Task<IActionResult> Index(int? jobId)
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

            IEnumerable<ApplicationResponseDTO>? applications;

            if (jobId.HasValue)
            {
                applications = await _applicationService.GetApplicationsByJobAsync(jobId.Value);
                var job = await _jobService.GetJobByIdAsync(jobId.Value);
                ViewBag.JobTitle = job?.Title ?? "Unknown Job";
                ViewBag.JobId = jobId.Value;
            }
            else
            {
                // Get all applications for all employer's jobs
                var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);
                if (employer != null)
                {
                    var jobs = await _jobService.GetJobsByEmployerAsync(employer.EmployerId);
                    var allApplications = new List<ApplicationResponseDTO>();
                    
                    if (jobs != null)
                    {
                        foreach (var job in jobs)
                        {
                            var jobApps = await _applicationService.GetApplicationsByJobAsync(job.JobId);
                            if (jobApps != null)
                            {
                                allApplications.AddRange(jobApps);
                            }
                        }
                    }
                    applications = allApplications;
                }
                else
                {
                    applications = Enumerable.Empty<ApplicationResponseDTO>();
                }
            }

            return View(applications ?? Enumerable.Empty<ApplicationResponseDTO>());
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var application = await _applicationService.GetApplicationByIdAsync(id);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }

        // POST: Applications/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _applicationService.UpdateApplicationStatusAsync(id, "Accepted");

            if (result)
            {
                TempData["SuccessMessage"] = "Application accepted! You can now create a contract.";
                return RedirectToAction("Create", "Contracts", new { applicationId = id });
            }

            TempData["ErrorMessage"] = "Failed to accept application.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Applications/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _applicationService.UpdateApplicationStatusAsync(id, "Rejected");

            if (result)
            {
                TempData["SuccessMessage"] = "Application rejected.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject application.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
