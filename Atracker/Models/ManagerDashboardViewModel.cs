using System.Collections.Generic;

namespace Atracker.Models
{
    public class TrackerInfoViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; } // We'll add this from tasks later
    }

    public class ManagerDashboardViewModel
    {
        public List<TrackerInfoViewModel> Trackers { get; set; }

        public ManagerDashboardViewModel()
        {
            Trackers = new List<TrackerInfoViewModel>();
        }
    }
}