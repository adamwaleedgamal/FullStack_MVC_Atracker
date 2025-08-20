using Atracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Fixes List<> error
using System.Linq; // Fixes .ToList(), .Select() etc. errors
using System.Threading.Tasks; // Fixes Task<> error

namespace Atracker.Controllers
{
    [Authorize(Roles = "Admin,Head")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) { _userManager = userManager; _roleManager = roleManager; }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users) { userViewModels.Add(new UserViewModel { UserId = user.Id, FullName = user.FullName, Email = user.Email, Roles = await _userManager.GetRolesAsync(user) }); }
            return View(userViewModels);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var viewModel = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Roles = allRoles.Select(role => new RoleViewModel
                {
                    RoleName = role.Name,
                    IsSelected = userRoles.Contains(role.Name) // THIS IS THE FIX for the 'await' error
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName);
            await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            return RedirectToAction("Index");
        }
    }
}