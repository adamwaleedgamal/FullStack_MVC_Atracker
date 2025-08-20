using System.Collections.Generic;
using System.Linq;

namespace Atracker.Models
{
    public class TrackerDashboardViewModel
    {
        public IEnumerable<TaskAssignment> TasksToday { get; set; }
        public IEnumerable<TaskAssignment> TasksLastWeek { get; set; }
        public IEnumerable<TaskAssignment> TasksLastMonth { get; set; }

        public TrackerDashboardViewModel()
        {
            TasksToday = Enumerable.Empty<TaskAssignment>();
            TasksLastWeek = Enumerable.Empty<TaskAssignment>();
            TasksLastMonth = Enumerable.Empty<TaskAssignment>();
        }
    }
}