// In Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace Atracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string? ManagerFeedback { get; set; }
        public ICollection<TaskItem> AssignedTasks { get; set; }
        // --- NEW PROPERTIES FOR PROFILE PAGE ---
        public DateTime? DateOfBirth { get; set; }
        public string? PermanentAddress { get; set; }
        public string? PresentAddress { get; set; }
        public string? ReferralSource { get; set; }
        public string? Gender { get; set; }
    }
}