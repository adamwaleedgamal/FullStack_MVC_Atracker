// In Controllers/ManagerController.cs

using Atracker.Data;
using Atracker.Models;
using Atracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    [Authorize(Roles = "Manager, Admin")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- USERS & TASK MANAGEMENT ---
        public async Task<IActionResult> UserTaskManagement(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var trackers = await _userManager.GetUsersInRoleAsync("Tracker");
            if (!string.IsNullOrEmpty(searchString))
            {
                trackers = trackers.Where(u => u.FullName.Contains(searchString) || u.Email.Contains(searchString)).ToList();
            }
            return View(trackers.OrderBy(u => u.FullName));
        }

        public IActionResult AddTracker() => View();
        [HttpPost]
        public async Task<IActionResult> AddTracker(ApplicationUser model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName, PhoneNumber = model.PhoneNumber, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, "Password@123");
            if (result.Succeeded) await _userManager.AddToRoleAsync(user, "Tracker");
            return RedirectToAction(nameof(UserTaskManagement));
        }

        public async Task<IActionResult> EditTracker(string id) => View(await _userManager.FindByIdAsync(id));
        [HttpPost]
        public async Task<IActionResult> EditTracker(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(UserTaskManagement));
        }

        public async Task<IActionResult> DeleteTracker(string id) => View(await _userManager.FindByIdAsync(id));
        [HttpPost, ActionName("DeleteTrackerConfirmed")]
        public async Task<IActionResult> DeleteTrackerConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null) await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(UserTaskManagement));
        }

        public async Task<IActionResult> Feedback(string id) => View(await _userManager.FindByIdAsync(id));
        [HttpPost]
        public async Task<IActionResult> Feedback(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            user.ManagerFeedback = model.ManagerFeedback;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(UserTaskManagement));
        }

        [HttpPost] public IActionResult BulkFeedback(string[] selectedUserIds) { /* Logic to process multiple users would go here */ return RedirectToAction(nameof(UserTaskManagement)); }


        // --- FLEET & VEHICLE MONITORING ---
        public async Task<IActionResult> FleetVehicleMonitoring() => View(await _context.Vehicles.Include(v => v.AssignedDriver).ToListAsync());

        public async Task<IActionResult> VehicleDetails(int id)
        {
            var vehicle = await _context.Vehicles.Include(v => v.AssignedDriver).Include(v => v.MaintenanceLogs).FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        public async Task<IActionResult> AddVehicle()
        {
            await PopulateDriversDropDownList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(FleetVehicleMonitoring));
            }
            await PopulateDriversDropDownList();
            return View(vehicle);
        }

        public async Task<IActionResult> EditVehicle(int id)
        {
            await PopulateDriversDropDownList();
            return View(await _context.Vehicles.FindAsync(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(int id, Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(FleetVehicleMonitoring));
            }
            await PopulateDriversDropDownList();
            return View(vehicle);
        }

        public async Task<IActionResult> DeleteVehicle(int id) => View(await _context.Vehicles.FindAsync(id));
        [HttpPost, ActionName("DeleteVehicleConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicleConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null) _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(FleetVehicleMonitoring));
        }


        // --- PERFORMANCE ---
        public async Task<IActionResult> Performance()
        {
            var viewModel = new PerformanceDashboardViewModel
            {
                ActiveUsers = (await _userManager.GetUsersInRoleAsync("Tracker")).Count(),
                TasksCompletedTotal = await _context.TaskItems.CountAsync(t => t.TaskStatus == Status.Completed),
            };
            return View(viewModel);
        }


        // --- REPORTS PAGE ---
        public async Task<IActionResult> AllReports()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var viewModel = new ManagerReportViewModel
            {
                // Get tasks with a deadline of today
                TodaysTasks = await _context.TaskItems
                    .Where(t => t.Deadline.Date == today).ToListAsync(),

                // Get tasks from the beginning of this week up to yesterday
                LastWeeksTasks = await _context.TaskItems
                    .Where(t => t.Deadline.Date >= startOfWeek && t.Deadline.Date < today).ToListAsync(),

                // Get tasks from the beginning of this month up to the start of this week
                LastMonthsTasks = await _context.TaskItems
                    .Where(t => t.Deadline.Date >= startOfMonth && t.Deadline.Date < startOfWeek).ToListAsync()
            };

            return View(viewModel);
        }


        // --- HELPER METHOD (NO LONGER DUPLICATED) ---
        private async Task PopulateDriversDropDownList()
        {
            var drivers = await _userManager.GetUsersInRoleAsync("Tracker");
            ViewBag.Drivers = new SelectList(drivers, "Id", "FullName");
        }
    }
}