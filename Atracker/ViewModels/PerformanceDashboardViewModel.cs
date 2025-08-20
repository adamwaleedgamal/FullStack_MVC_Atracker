// In ViewModels/PerformanceDashboardViewModel.cs

using System.Collections.Generic;

namespace Atracker.ViewModels
{
    // THIS IS THE CORRECT VIEWMODEL FOR THE PERFORMANCE PAGE
    // It contains all the properties that the view is trying to find.
    public class PerformanceDashboardViewModel
    {
        public int ActiveUsers { get; set; }
        public int TotalUsersGoal { get; set; } = 50;
        public int TasksCompletedTotal { get; set; }
        public string EarningsAndIncentives { get; set; } = "2m 34s";
        public int StartingKnowledgePercent { get; set; } = 64;
        public int CurrentKnowledgePercent { get; set; } = 86;
        public string KnowledgeGain => $"+{CurrentKnowledgePercent - StartingKnowledgePercent}%";
        public List<TopPerformer> TopPerformers { get; set; }
        public int CompletedTasksFastPercent { get; set; } = 72;
        public int CompletedTasksMediumPercent { get; set; } = 68;
        public int CompletedTasksSlowPercent { get; set; } = 58;

        public PerformanceDashboardViewModel()
        {
            TopPerformers = new List<TopPerformer>
            {
                new TopPerformer { Name = "delivery rip 1", Score = 74 },
                new TopPerformer { Name = "delivery rip 2", Score = 52 },
                new TopPerformer { Name = "delivery rip 3", Score = 26 }
            };
        }
    }

    public class TopPerformer
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}