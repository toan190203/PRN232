using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models.DTOs;
using PartTimeJobManagement.Client.Services;

namespace PartTimeJobManagement.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result != null)
            {
                TempData["SuccessMessage"] = $"Welcome back, {result.Email}!";
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirect based on role
                return result.Role switch
                {
                    "Student" => RedirectToAction("Index", "Jobs"),
                    "Employer" => RedirectToAction("MyJobs", "Jobs"),
                    "Admin" => RedirectToAction("Dashboard", "Admin"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Registration successful! Welcome!";
                
                return result.Role switch
                {
                    "Student" => RedirectToAction("Index", "Jobs"),
                    "Employer" => RedirectToAction("MyJobs", "Jobs"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", "Registration failed. Email may already be in use.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
