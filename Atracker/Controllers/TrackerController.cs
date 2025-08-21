// In Controllers/TrackerController.cs

using Atracker.Data;
using Atracker.Models;
using Atracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    [Authorize(Roles = "Tracker, Admin")]
    public class TrackerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrackerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> TaskOverview()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            var currentTask = await _context.TaskItems.FirstOrDefaultAsync(t => t.AssignedToId == userId && t.TaskStatus != Status.Completed);
            var assignedCar = await _context.Vehicles.FirstOrDefaultAsync(v => v.AssignedDriverId == userId);

            var viewModel = new TaskOverviewViewModel
            {
                CurrentUser = currentUser,
                CurrentTask = currentTask,
                AssignedCar = assignedCar
            };
            return View(viewModel);
        }

        // --- VEHICLE & PERFORMANCE MONITORING ---
        public IActionResult VehiclePerformanceMonitoring() => View(new DailyReport());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VehiclePerformanceMonitoring([Bind("ReportContent")] DailyReport report)
        {
            if (ModelState.IsValid)
            {
                report.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                report.ReportDate = DateTime.UtcNow;
                _context.Add(report);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your daily report has been submitted successfully!";
                return RedirectToAction(nameof(TaskOverview));
            }
            return View(report);
        }

        public async Task<IActionResult> UpdateCarData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignedVehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.AssignedDriverId == userId);
            return View(assignedVehicle);
        }
    }
}