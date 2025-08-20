using System.Collections.Generic; // Add this
using System.Linq; // Add this

namespace Atracker.Models
{
    public class HeadDashboardViewModel
    {
        public List<TrackerInfoViewModel> Trackers { get; set; }
        public List<TaskAssignment> RecentTasks { get; set; }
        public HeadDashboardViewModel() { Trackers = new List<TrackerInfoViewModel>(); RecentTasks = new List<TaskAssignment>(); }
    }
}