using Atracker.Data;
using Atracker.Models;
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
    [Authorize]
    public class TaskManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= CREATE TASK =================
        [Authorize(Roles = "Admin,Head,Manager")]
        public async Task<IActionResult> Create()
        {
            var trackers = await _userManager.GetUsersInRoleAsync("Tracker");
            var viewModel = new CreateTaskViewModel
            {
                TrackerList = trackers.Select(t => new SelectListItem
                {
                    Text = t.FullName,
                    Value = t.Id
                }),
                Deadline = DateTime.Now.AddDays(1)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Manager")]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var assignedBy = await _userManager.GetUserAsync(User);
                var task = new TaskAssignment
                {
                    Title = model.Title,
                    Description = model.Description,
                    Deadline = model.Deadline,
                    FromLocation = model.FromLocation,
                    ToLocation = model.ToLocation,
                    Priority = model.Priority,
                    Status = AssignmentStatus.Assigned,
                    AssignedToId = model.AssignedToId,
                    AssignedById = assignedBy.Id,
                    CreatedDate = DateTime.UtcNow
                };

                _context.TaskAssignments.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Dashboard");
            }

            var trackers = await _userManager.GetUsersInRoleAsync("Tracker");
            model.TrackerList = trackers.Select(t => new SelectListItem
            {
                Text = t.FullName,
                Value = t.Id
            });

            return View(model);
        }

        // ================= TASK DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.TaskAssignments
                .Include(t => t.AssignedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            var viewModel = new TaskDetailViewModel
            {
                Task = task,
                AssignedByName = task.AssignedBy?.FullName ?? "N/A",
                UpdateForm = new UpdateTaskViewModel
                {
                    TaskId = task.Id,
                    NewStatus = task.Status
                }
            };

            return View(viewModel);
        }

        // ================= UPDATE TASK STATUS =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTaskStatus(UpdateTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { id = model.TaskId });

            var taskToUpdate = await _context.TaskAssignments.FindAsync(model.TaskId);
            if (taskToUpdate == null) return NotFound();

            taskToUpdate.Status = model.NewStatus;

            if (!string.IsNullOrWhiteSpace(model.ReplyText))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var feedback = new TaskFeedback
                {
                    TaskId = taskToUpdate.Id,
                    Content = model.ReplyText,
                    SubmittedById = currentUser.Id,
                    SubmittedDate = DateTime.UtcNow
                };

                _context.TaskFeedbacks.Add(feedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
