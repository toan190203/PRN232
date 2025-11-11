using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Services;
using PartTimeJobManagement.Client.Models.DTOs;

namespace PartTimeJobManagement.Client.Controllers
{
    public class RolesController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRoleService _roleService;

        public RolesController(IAuthService authService, IRoleService roleService)
        {
            _authService = authService;
            _roleService = roleService;
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

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleDTO createRoleDto)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (!ModelState.IsValid)
            {
                return View(createRoleDto);
            }

            var result = await _roleService.CreateRoleAsync(createRoleDto);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Role created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create role. Role name may already exist.";
            return View(createRoleDto);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UpdateRoleDTO
            {
                RoleName = role.RoleName
            };

            ViewBag.RoleId = role.RoleId;
            return View(updateDto);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRoleDTO updateRoleDto)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = id;
                return View(updateRoleDto);
            }

            var result = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            if (result)
            {
                TempData["SuccessMessage"] = "Role updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to update role.";
            ViewBag.RoleId = id;
            return View(updateRoleDto);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var result = await _roleService.DeleteRoleAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Role deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete role. Make sure no users are assigned to this role.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
