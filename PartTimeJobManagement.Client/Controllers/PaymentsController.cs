using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models.DTOs;
using PartTimeJobManagement.Client.Services;

namespace PartTimeJobManagement.Client.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPaymentService _paymentService;
        private readonly IContractService _contractService;
        private readonly IStudentService _studentService;

        public PaymentsController(IAuthService authService, IPaymentService paymentService, IContractService contractService, IStudentService studentService)
        {
            _authService = authService;
            _paymentService = paymentService;
            _contractService = contractService;
            _studentService = studentService;
        }

        // GET: Payments?contractId=5 (for employers)
        public async Task<IActionResult> Index(int? contractId)
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

            IEnumerable<PaymentResponseDTO>? payments;

            if (contractId.HasValue)
            {
                payments = await _paymentService.GetPaymentsByContractAsync(contractId.Value);
                ViewBag.ContractId = contractId.Value;
            }
            else
            {
                // Không gọi endpoint Admin-only. Gom theo tất cả hợp đồng của Employer.
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var contracts = await _contractService.GetContractsByEmployerAsync(userId.Value);
                var list = new List<PaymentResponseDTO>();
                if (contracts != null)
                {
                    foreach (var c in contracts)
                    {
                        var items = await _paymentService.GetPaymentsByContractAsync(c.ContractId);
                        if (items != null) list.AddRange(items);
                    }
                }
                payments = list;
            }

            return View(payments ?? Enumerable.Empty<PaymentResponseDTO>());
        }

        // GET: Payments/Create?contractId=5
        public async Task<IActionResult> Create(int? contractId)
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

            if (!contractId.HasValue)
            {
                TempData["ErrorMessage"] = "Contract ID is required.";
                return RedirectToAction("Index", "Contracts");
            }

            var contract = await _contractService.GetContractByIdAsync(contractId.Value);
            if (contract == null)
            {
                TempData["ErrorMessage"] = "Contract not found.";
                return RedirectToAction("Index", "Contracts");
            }

            ViewBag.Contract = contract;

            return View(new CreatePaymentDTO 
            { 
                ContractId = contractId.Value
            });
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePaymentDTO model)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var contract = await _contractService.GetContractByIdAsync(model.ContractId);
                ViewBag.Contract = contract;
                return View(model);
            }

            var result = await _paymentService.CreatePaymentAsync(model);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Payment created successfully!";
                return RedirectToAction(nameof(Index), new { contractId = model.ContractId });
            }

            TempData["ErrorMessage"] = "Failed to create payment.";
            return View(model);
        }

        // GET: Payments/MyPayments (for students)
        public async Task<IActionResult> MyPayments()
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
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var payments = await _paymentService.GetPaymentsByStudentAsync(student.StudentId);

            return View(payments ?? Enumerable.Empty<PaymentResponseDTO>());
        }

        // POST: Payments/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, int? contractId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _authService.GetCurrentUserRole();
            if (role != "Employer" && role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var updateDto = new UpdatePaymentDTO
            {
                Status = status
            };

            var result = await _paymentService.UpdatePaymentAsync(id, updateDto);

            if (result)
            {
                TempData["SuccessMessage"] = $"Payment status updated to {status}!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update payment status.";
            }

            if (contractId.HasValue)
            {
                return RedirectToAction(nameof(Index), new { contractId = contractId.Value });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Payments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _paymentService.DeletePaymentAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Payment deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete payment.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
