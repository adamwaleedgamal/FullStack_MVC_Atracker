using Atracker.Data;
using Atracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atracker.Documents;
using QuestPDF.Fluent;

namespace Atracker.Controllers
{
    [Authorize(Roles = "Admin,Head,Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) { _context = context; _userManager = userManager; }

        public async Task<IActionResult> Index()
        {
            var vm = new PerformanceReportViewModel();
            vm.TasksCompleted = await _context.TaskAssignments.CountAsync(t => t.Status == AssignmentStatus.Completed); // --- USING THE NEW NAME HERE ---
            vm.TopPerformers = await _context.TaskAssignments
                .Where(t => t.Status == AssignmentStatus.Completed) // --- USING THE NEW NAME HERE ---
                .Include(t => t.AssignedTo).GroupBy(t => t.AssignedTo)
                .Select(g => new TopPerformerViewModel { Name = g.Key.FullName, CompletedTasks = g.Count() })
                .OrderByDescending(x => x.CompletedTasks).Take(3).ToListAsync();
            // ... the rest of the code is correct
            return View(vm);
        }

        public async Task<IActionResult> DownloadPdf()
        {
            var vm = new PerformanceReportViewModel();
            vm.TasksCompleted = await _context.TaskAssignments.CountAsync(t => t.Status == AssignmentStatus.Completed); // --- USING THE NEW NAME HERE ---
            // ... the rest of the code is correct
            var document = new PerformanceReportDocument(vm);
            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"PerformanceReport-{DateTime.Now:yyyy-MM-dd}.pdf");
        }
    }
}