using Atracker.Data;
using Atracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) { _userManager = userManager; _context = context; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var trackersInRole = await _userManager.GetUsersInRoleAsync("Tracker");
                var viewModel = new ManagerDashboardViewModel();
                foreach (var trackerUser in trackersInRole) { viewModel.Trackers.Add(new TrackerInfoViewModel { UserId = trackerUser.Id, FullName = trackerUser.FullName, Email = trackerUser.Email, PhoneNumber = trackerUser.PhoneNumber ?? "N/A", Status = "Idle" }); }
                return View("ManagerDashboard", viewModel);
            }

            if (await _userManager.IsInRoleAsync(user, "Head"))
            {
                var headViewModel = new HeadDashboardViewModel();
                var trackers = await _userManager.GetUsersInRoleAsync("Tracker");
                foreach (var trackerUser in trackers) { headViewModel.Trackers.Add(new TrackerInfoViewModel { UserId = trackerUser.Id, FullName = trackerUser.FullName, Email = trackerUser.Email, PhoneNumber = trackerUser.PhoneNumber ?? "N/A", Status = "Idle" }); }
                headViewModel.RecentTasks = await _context.TaskAssignments.Include(t => t.AssignedTo).OrderByDescending(t => t.CreatedDate).Take(10).ToListAsync();
                return View("HeadDashboard", headViewModel);
            }

            if (await _userManager.IsInRoleAsync(user, "Tracker"))
            {
                var today = DateTime.UtcNow.Date;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var userTasks = await _context.TaskAssignments.Where(t => t.AssignedToId == user.Id).OrderByDescending(t => t.CreatedDate).ToListAsync();
                var viewModel = new TrackerDashboardViewModel { TasksToday = userTasks.Where(t => t.CreatedDate.Date == today), TasksLastWeek = userTasks.Where(t => t.CreatedDate >= startOfWeek && t.CreatedDate.Date != today), TasksLastMonth = userTasks.Where(t => t.CreatedDate >= startOfMonth && t.CreatedDate < startOfWeek) };
                return View("TrackerDashboard", viewModel);
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return View("AdminDashboard");
            }

            return View("NoRoleDashboard");
        }
    }
}