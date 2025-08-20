using System.Collections.Generic;
namespace Atracker.Models

{
    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}