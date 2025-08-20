using System.ComponentModel.DataAnnotations;

namespace Atracker.Models
{
    public class UserEditViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}