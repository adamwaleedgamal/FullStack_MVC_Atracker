// In Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace Atracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string? ManagerFeedback { get; set; }
    }
}