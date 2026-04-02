using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.ViewModels.Admin;
using static StudyPlanner.GCommon.ApplicationConstants;

namespace StudyPlanner.Web.Areas.Admin.Controllers
{
    [Area(AdminAreaName)]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int PageSize = 10;

        public UsersController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1)
        {
            var (dtos, totalCount) = await _adminService.GetAllUsersAsync(searchTerm, pageNumber, PageSize);

            var viewModels = dtos.Select(d => new AdminUserViewModel
            {
                Id = d.Id.ToString(),
                Email = d.Email,
                FullName = d.FullName
            }).ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            ViewBag.TotalCount = totalCount;

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dto = await _adminService.GetUserByIdAsync(id);

                var viewModel = new AdminUserViewModel
                {
                    Id = dto.Id.ToString(),
                    Email = dto.Email,
                    FullName = dto.FullName
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (id.ToString() == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _adminService.DeleteUserAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}