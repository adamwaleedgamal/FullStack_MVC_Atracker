// In ViewModels/TaskOverviewViewModel.cs
using Atracker.Models;

namespace Atracker.ViewModels
{
    public class TaskOverviewViewModel
    {
        public TaskItem CurrentTask { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public Vehicle AssignedCar { get; set; }
    }
}