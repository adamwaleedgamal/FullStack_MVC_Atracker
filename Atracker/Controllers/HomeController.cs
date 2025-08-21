using Atracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Atracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("TaskDeliveryManagement", "Admin"); 
            }
            if (User.IsInRole("Manager"))
            {
                return RedirectToAction("UserTaskManagement", "Manager");
            }
            if (User.IsInRole("Head"))
            {
                return RedirectToAction("UserManagement", "Head");
            }
            if (User.IsInRole("Tracker"))
            {
                return RedirectToAction("TaskOverview", "Tracker");
            }

            return View(); // Fallback
        }

    }
}