// In ViewModels/TrackerDashboardViewModel.cs

using Atracker.Models;
using System.Collections.Generic;

// THIS LINE IS CRITICAL. It tells the rest of the application where to find this class.
namespace Atracker.ViewModels
{
    public class TrackerDashboardViewModel
    {
        public List<TaskItem> TodaysTasks { get; set; }
        public List<TaskItem> LastWeeksTasks { get; set; }
        public List<TaskItem> LastMonthsTasks { get; set; }
    }
}