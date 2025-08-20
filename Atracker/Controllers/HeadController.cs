// In Controllers/HeadController.cs

using Atracker.Data;
using Atracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    [Authorize(Roles = "Head")]
    public class HeadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HeadController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- MAIN DASHBOARD (USER MANAGEMENT) ---
        // This action now serves as the main dashboard, displaying the task list.
        // GET: /Head/UserManagement
        public async Task<IActionResult> UserManagement()
        {
            var tasks = await _context.TaskItems.ToListAsync();
            return View(tasks);
        }

        // --- TASK CRUD ---
        public IActionResult AddTask() => View();
        [HttpPost]
        public async Task<IActionResult> AddTask(TaskItem task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserManagement));
            }
            return View(task);
        }

        public async Task<IActionResult> EditTask(int? id)
        {
            if (id == null) return NotFound();
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int id, [Bind("Id,Title,Description,Deadline,Location,DeliveryPriority,TaskStatus,ReplyMessage")] TaskItem task)
        {
            if (id != task.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserManagement));
            }
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
            if (task != null)
            {
                _context.TaskItems.Remove(task);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }

        // --- FEEDBACK FUNCTION ---
        // GET: /Head/Feedback/5
        public async Task<IActionResult> Feedback(int? id) // Action name matches the button link
        {
            if (id == null) return NotFound();
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        // POST: /Head/Feedback/5
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


        // --- WAREHOUSE SAMPLES CRUD ---
        public async Task<IActionResult> WarehouseData()
        {
            if (!_context.WarehouseSamples.Any())
            {
                _context.WarehouseSamples.Add(new WarehouseSample { SampleName = "Chemical A", SampleIdentifier = "CHEM-001" });
                _context.WarehouseSamples.Add(new WarehouseSample { SampleName = "Component B", SampleIdentifier = "COMP-B-42" });
                await _context.SaveChangesAsync();
            }
            return View(await _context.WarehouseSamples.ToListAsync());
        }
        public IActionResult AddSample() => View();
        [HttpPost]
        public async Task<IActionResult> AddSample(WarehouseSample sample)
        {
            _context.Add(sample);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(WarehouseData));
        }
        public async Task<IActionResult> EditSample(int? id) => View(await _context.WarehouseSamples.FindAsync(id));
        [HttpPost]
        public async Task<IActionResult> EditSample(int id, WarehouseSample sample)
        {
            _context.Update(sample);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(WarehouseData));
        }
        public async Task<IActionResult> DeleteSample(int? id) => View(await _context.WarehouseSamples.FindAsync(id));
        [HttpPost, ActionName("DeleteSampleConfirmed")]
        public async Task<IActionResult> DeleteSampleConfirmed(int id)
        {
            var sample = await _context.WarehouseSamples.FindAsync(id);
            if (sample != null) _context.WarehouseSamples.Remove(sample);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(WarehouseData));
        }


        // --- OTHER SIDEBAR PAGES ---
        public async Task<IActionResult> ViewAllUsers() => View(await _userManager.Users.ToListAsync());
        public IActionResult Scheduling() => View();
        public IActionResult Performance() => View();
        public IActionResult Communication() => View();
    }
}