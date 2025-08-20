using Atracker.Data;
using Atracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Atracker.Controllers
{
    // Only Managers, Heads, and Admins should be able to see the fleet.
    [Authorize(Roles = "Manager,Head,Admin")]
    public class FleetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FleetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Fleet or /Fleet/Index
        // This action will list all vehicles in the fleet.
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return View(vehicles);
        }

        // GET: /Fleet/Details/5
        // This action shows the monitoring page for a single vehicle.
        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.MaintenanceLogs) // Load related maintenance data
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            // Let's find the current task and driver for this vehicle
            var currentTask = await _context.TaskAssignments
                .Include(t => t.AssignedTo) // Load the driver's details
                .Where(t => t.VehicleId == id && t.Status != AssignmentStatus.Completed)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefaultAsync();

            // We'll pass the additional data to the view using the ViewBag
            ViewBag.CurrentDriver = currentTask?.AssignedTo;
            ViewBag.CurrentTask = currentTask;

            return View(vehicle);
        }

        // GET: /Fleet/Create
        // This action just displays the empty form to add a new vehicle.
        public IActionResult Create()
        {
            // We pass a new empty Vehicle object to the view.
            return View(new Vehicle());
        }

        // POST: /Fleet/Create
        // This action handles the data submitted from the form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            // Check if the submitted data (CarType, LicensePlate) is valid
            if (ModelState.IsValid)
            {
                // Add the new vehicle to the database context
                _context.Vehicles.Add(vehicle);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Redirect the user back to the main fleet list page
                return RedirectToAction(nameof(Index));
            }

            // If the data was not valid, return the user to the form so they can correct it.
            return View(vehicle);
        }
    }
}