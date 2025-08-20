// In Models/PerformanceReportViewModel.cs
namespace Atracker.Models
{
    // A simple class to hold data for the "Top 3" list
    public class TopPerformerViewModel
    {
        public string Name { get; set; }
        public int CompletedTasks { get; set; }
    }

    public class PerformanceReportViewModel
    {
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
        public int TasksCompleted { get; set; }
        public string AverageCompletionTime { get; set; } // We'll represent this as a string for now

        public List<TopPerformerViewModel> TopPerformers { get; set; }

        public List<string> WeeklyTaskChartLabels { get; set; }
        public List<int> WeeklyTaskChartData { get; set; }

        public PerformanceReportViewModel()
        {
            TopPerformers = new List<TopPerformerViewModel>();
            WeeklyTaskChartLabels = new List<string>();
            WeeklyTaskChartData = new List<int>();
        }
    }
}