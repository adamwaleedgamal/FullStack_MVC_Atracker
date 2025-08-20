// In ViewModels/ManagerReportViewModel.cs

using Atracker.Models;
using System.Collections.Generic;

namespace Atracker.ViewModels
{
    public class ManagerReportViewModel
    {
        public List<TaskItem> TodaysTasks { get; set; }
        public List<TaskItem> LastWeeksTasks { get; set; }
        public List<TaskItem> LastMonthsTasks { get; set; }
    }
}