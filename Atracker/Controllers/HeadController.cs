// In Controllers/HeadController.cs

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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    [Authorize(Roles = "Head, Admin")]
    public class HeadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HeadController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- USERS MANAGEMENT (MAIN DASHBOARD) & TASK CRUD ---
        public async Task<IActionResult> UserManagement(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var tasksQuery = _context.TaskItems.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchString));
            }
            return View(await tasksQuery.OrderByDescending(t => t.Id).ToListAsync());
        }

        public async Task<IActionResult> AddTask()
        {
            await PopulateAssignableUsersDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask([Bind("Title,Description,Deadline,Location,DeliveryPriority,AssignedToId")] TaskItem task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserManagement));
            }
            await PopulateAssignableUsersDropDownList();
            return View(task);
        }

        public async Task<IActionResult> EditTask(int? id)
        {
            if (id == null) return NotFound();
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            await PopulateAssignableUsersDropDownList();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int id, [Bind("Id,Title,Description,Deadline,Location,DeliveryPriority,TaskStatus,ReplyMessage,AssignedToId")] TaskItem task)
        {
            if (id != task.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserManagement));
            }
            await PopulateAssignableUsersDropDownList();
            return View(task);
        }

        public async Task<IActionResult> DeleteTask(int? id)
        {
            if (id == null) return NotFound();
            var task = await _context.TaskItems.FirstOrDefaultAsync(m => m.Id == id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost, ActionName("DeleteTaskConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTaskConfirmed(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task != null) _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }

        public async Task<IActionResult> Feedback(int? id)
        {
            if (id == null) return NotFound();
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(int id, string replyMessage)
        {
            var taskToUpdate = await _context.TaskItems.FindAsync(id);
            if (taskToUpdate == null) return NotFound();
            taskToUpdate.ReplyMessage = replyMessage;
            _context.Update(taskToUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }

        // --- BULK FEEDBACK ---
        [HttpPost]
        public async Task<IActionResult> BulkFeedback(int[] selectedTaskIds, string feedbackMessage)
        {
            if (selectedTaskIds == null || selectedTaskIds.Length == 0) return RedirectToAction(nameof(UserManagement));

            if (string.IsNullOrEmpty(feedbackMessage))
            {
                // This now needs a uniquely named ViewModel
                var viewModel = new BulkFeedbackViewModel { SelectedTaskIds = selectedTaskIds.ToList() };
                return View(viewModel);
            }

            var tasksToUpdate = await _context.TaskItems.Where(t => selectedTaskIds.Contains(t.Id)).ToListAsync();
            foreach (var task in tasksToUpdate)
            {
                task.ReplyMessage = feedbackMessage;
            }
            _context.UpdateRange(tasksToUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }

        // --- SCHEDULING ---
        public async Task<IActionResult> Scheduling()
        {
            await PopulateAssignableUsersDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scheduling([Bind("Title,Description,Deadline,Location,DeliveryPriority,AssignedToId")] TaskItem task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserManagement));
            }
            await PopulateAssignableUsersDropDownList();
            return View(task);
        }

        // --- PERFORMANCE & REPORTS ---
        public async Task<IActionResult> Performance()
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

        // --- COMMUNICATION & ISSUE REPORTING ---
        public IActionResult Communication() => View(new IssueReport());
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Communication(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var report = new IssueReport
                {
                    Content = content,
                    ReportedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ReportDate = DateTime.UtcNow
                };
                _context.Add(report);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your report has been submitted successfully!";
                return RedirectToAction(nameof(ViewReports));
            }
            ModelState.AddModelError("Content", "Please enter details for your report.");
            var invalidReport = new IssueReport { Content = content };
            return View(invalidReport);
        }

        public async Task<IActionResult> ViewReports()
        {
            var reports = await _context.IssueReports
                .Include(r => r.ReportedBy)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
            return View(reports);
        }

        // --- VIEW TRACKER'S DAILY REPORTS ---
        // Inside HeadController.cs

        public async Task<IActionResult> ViewDailyReports()
        {
            var reports = await _context.DailyReports
                .Include(r => r.User) // Include the user's details to show their name
                .OrderByDescending(r => r.ReportDate) // Show the newest reports first
                .ToListAsync();

            return View(reports);
        }

        // --- WAREHOUSE DATA (FULL CRUD) ---
        public async Task<IActionResult> WarehouseData() => View(await _context.WarehouseSamples.ToListAsync());
        public IActionResult AddSample() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSample([Bind("SampleName,SampleIdentifier")] WarehouseSample sample)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sample);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(WarehouseData));
            }
            return View(sample);
        }

        public async Task<IActionResult> EditSample(int? id)
        {
            if (id == null) return NotFound();
            var sample = await _context.WarehouseSamples.FindAsync(id);
            if (sample == null) return NotFound();
            return View(sample);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSample(int id, [Bind("Id,SampleName,SampleIdentifier")] WarehouseSample sample)
        {
            if (id != sample.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(sample);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(WarehouseData));
            }
            return View(sample);
        }

        public async Task<IActionResult> DeleteSample(int? id)
        {
            if (id == null) return NotFound();
            var sample = await _context.WarehouseSamples.FindAsync(id);
            if (sample == null) return NotFound();
            return View(sample);
        }

        [HttpPost, ActionName("DeleteSampleConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSampleConfirmed(int id)
        {
            var sample = await _context.WarehouseSamples.FindAsync(id);
            if (sample != null) _context.WarehouseSamples.Remove(sample);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(WarehouseData));
        }

        // --- HELPER METHOD ---
        private async Task PopulateAssignableUsersDropDownList()
        {
            var managers = await _userManager.GetUsersInRoleAsync("Manager");
            var trackers = await _userManager.GetUsersInRoleAsync("Tracker");
            var assignableUsers = managers.Concat(trackers).OrderBy(u => u.FullName).ToList();
            ViewBag.AssignableUsers = new SelectList(assignableUsers, "Id", "FullName");
        }
    }
}