using System.Collections.Generic;
namespace Atracker.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}