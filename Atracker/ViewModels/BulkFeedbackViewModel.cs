// In ViewModels/BulkFeedbackViewModel.cs
using System.Collections.Generic;
namespace Atracker.ViewModels
{
    public class BulkFeedbackViewModel
    {
        public List<int> SelectedTaskIds { get; set; } = new List<int>();
        public string FeedbackMessage { get; set; }
    }
}