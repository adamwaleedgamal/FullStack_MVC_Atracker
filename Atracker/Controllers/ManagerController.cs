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

        // --- USERS & TASK MANAGEMENT (WITH SEARCH) ---
        public async Task<IActionResult> UserTaskManagement(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var trackers = await _userManager.GetUsersInRoleAsync("Tracker");

            if (!string.IsNullOrEmpty(searchString))
            {
                trackers = trackers.Where(u => u.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) || u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return View(trackers.OrderBy(u => u.FullName));
        }

        // --- USER CRUD ---
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrackerConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null) await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(UserTaskManagement));
        }

        // --- FEEDBACK ---
        public async Task<IActionResult> Feedback(string id) => View(await _userManager.FindByIdAsync(id));
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            user.ManagerFeedback = model.ManagerFeedback;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(UserTaskManagement));
        }

        [HttpPost]
        public async Task<IActionResult> BulkFeedback(string[] selectedUserIds, string feedbackMessage)
        {
            if (selectedUserIds == null || selectedUserIds.Length == 0) return RedirectToAction(nameof(UserTaskManagement));

            if (string.IsNullOrEmpty(feedbackMessage))
            {
                // THIS IS THE CRITICAL CHANGE
                // We are now creating and using the new, correctly named ViewModel
                var viewModel = new ManagerBulkFeedbackViewModel
                {
                    SelectedUserIds = selectedUserIds.ToList()
                };
                return View(viewModel);
            }

            var usersToUpdate = await _userManager.Users.Where(u => selectedUserIds.Contains(u.Id)).ToListAsync();
            foreach (var user in usersToUpdate)
            {
                user.ManagerFeedback = feedbackMessage;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(UserTaskManagement));
        }


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
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(FleetVehicleMonitoring));
            }
            await PopulateDriversDropDownList();
            return View(vehicle);
        }

        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }
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
                TodaysTasks = await _context.TaskItems.Where(t => t.Deadline.Date == today).ToListAsync(),
                LastWeeksTasks = await _context.TaskItems.Where(t => t.Deadline.Date >= startOfWeek && t.Deadline.Date < today).ToListAsync(),
                LastMonthsTasks = await _context.TaskItems.Where(t => t.Deadline.Date >= startOfMonth && t.Deadline.Date < startOfWeek).ToListAsync()
            };
            return View(viewModel);
        }


        // --- HELPER METHOD ---
        private async Task PopulateDriversDropDownList()
        {
            var drivers = await _userManager.GetUsersInRoleAsync("Tracker");
            ViewBag.Drivers = new SelectList(drivers.OrderBy(d => d.FullName), "Id", "FullName");
        }
    }
}