// In Controllers/TrackerController.cs

using Atracker.Data;
using Atracker.ViewModels; // <-- THIS LINE MUST EXIST. It finds the namespace from Step 1.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    [Authorize(Roles = "Tracker, Admin")]
    public class TrackerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TrackerController(ApplicationDbContext context) { _context = context; }

        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            // ... (rest of the code is the same)
            var viewModel = new TrackerDashboardViewModel
            {
                TodaysTasks = await _context.TaskItems.Where(t => t.Deadline.Date == today).ToListAsync(),
                LastWeeksTasks = await _context.TaskItems.Where(t => t.Deadline.Date >= today.AddDays(-(int)today.DayOfWeek) && t.Deadline.Date < today).ToListAsync(),
                LastMonthsTasks = await _context.TaskItems.Where(t => t.Deadline.Date >= new DateTime(today.Year, today.Month, 1) && t.Deadline.Date < today.AddDays(-(int)today.DayOfWeek)).ToListAsync()
            };
            return View(viewModel);
        }

        // ... (other placeholder actions)
        public IActionResult UsersManagement() => View();
        public IActionResult SchedulingAndDistrib() => View();
        public IActionResult PerformanceAndReports() => View();
        public IActionResult CommunicationAndIssue() => View();
        public IActionResult AllWarehouseData() => View();
        public IActionResult Reply() => View();
    }
}