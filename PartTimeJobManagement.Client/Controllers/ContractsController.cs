using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models.DTOs;
using PartTimeJobManagement.Client.Services;

namespace PartTimeJobManagement.Client.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IContractService _contractService;
        private readonly IApplicationService _applicationService;
        private readonly IStudentService _studentService;
        private readonly IEmployerService _employerService;

        public ContractsController(
            IAuthService authService, 
            IContractService contractService, 
            IApplicationService applicationService, 
            IStudentService studentService,
            IEmployerService employerService)
        {
            _authService = authService;
            _contractService = contractService;
            _applicationService = applicationService;
            _studentService = studentService;
            _employerService = employerService;
        }

        // GET: Contracts (for employers)
        public async Task<IActionResult> Index()
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

            // Lấy Employer record để có EmployerId đúng
            var employer = await _employerService.GetEmployerByUserIdAsync(userId.Value);
            if (employer == null)
            {
                TempData["ErrorMessage"] = "Employer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var contracts = await _contractService.GetContractsByEmployerAsync(employer.EmployerId);

            return View(contracts ?? Enumerable.Empty<ContractResponseDTO>());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                TempData["ErrorMessage"] = "Contract not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(contract);
        }

        // GET: Contracts/Create?applicationId=5
        public async Task<IActionResult> Create(int? applicationId)
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

            if (!applicationId.HasValue)
            {
                TempData["ErrorMessage"] = "Application ID is required.";
                return RedirectToAction("Index", "Applications");
            }

            ViewBag.ApplicationId = applicationId.Value;

            return View(new CreateContractDTO { ApplicationId = applicationId.Value });
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContractDTO model)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _contractService.CreateContractAsync(model);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Contract created successfully!";
                return RedirectToAction(nameof(Details), new { id = result.ContractId });
            }

            TempData["ErrorMessage"] = "Failed to create contract.";
            return View(model);
        }

        // GET: Contracts/MyContracts (for students)
        public async Task<IActionResult> MyContracts()
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
                TempData["InfoMessage"] = "Please complete your student profile to view contracts.";
                ViewBag.NeedsProfile = true;
                return View(Enumerable.Empty<ContractResponseDTO>());
            }

            var contracts = await _contractService.GetContractsByStudentAsync(student.StudentId);

            if (contracts == null || !contracts.Any())
            {
                TempData["InfoMessage"] = "You don't have any contracts yet.";
            }

            return View(contracts ?? Enumerable.Empty<ContractResponseDTO>());
        }

        // POST: Contracts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _contractService.DeleteContractAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Contract deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete contract.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
