// In ViewModels/ManagerBulkFeedbackViewModel.cs

using System.Collections.Generic;

namespace Atracker.ViewModels
{
    // This is the new, correctly named ViewModel for the Manager's bulk feedback.
    public class ManagerBulkFeedbackViewModel
    {
        // This ViewModel works with User IDs, which are strings (guids)
        public List<string> SelectedUserIds { get; set; } = new List<string>();

        public string FeedbackMessage { get; set; }
    }
}